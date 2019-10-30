using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Linq;

namespace Utility
{
    public class FileUtility
    {
        private string _rootpath;
        private HashSet<string> allow_file_format = new HashSet<string> { "png", "jpg", "jpeg"};

        //Company name, Image url list
        private Dictionary<string, List<ImageData>> imageDict = new Dictionary<string, List<ImageData>>();

        public void SetTargetFolder(string targetFolder) {
            imageDict.Clear();
            _rootpath = targetFolder;

            LoadAllImagesFromFolder();
        }

        public ImageData GetRandomImage() {
            int keyCount = imageDict.Count;
            int random_companyIndex = Random.Range(0, keyCount);
            string key = imageDict.Keys.ElementAt(random_companyIndex);

            if (imageDict.TryGetValue(key, out List<ImageData> imageDataList)) {

                imageDataList = imageDataList.FindAll(x => !x.is_display);

                int image_index = Random.Range(0, imageDataList.Count);
                if (image_index <= 0)
                    return GetRandomImage();

                ImageData imageData = imageDataList.ElementAt(image_index);

                ReturnImageData(imageData, true);

                return imageData;
            }

            return default(ImageData);
        }

        public ImageData GetRandomImageByTag(string company, bool isPick)
        {

            if (!string.IsNullOrEmpty(company) && imageDict.TryGetValue(company, out List<ImageData> imageData))
            {
                imageData = imageData.FindAll(x => x.is_picked == isPick);
                int image_index = Random.Range(0, imageData.Count);
                return imageData.ElementAt(image_index);
            }

            return default(ImageData);
        }

        public void ReturnImageData(ImageData imageData, bool isDisplay) {

            if (imageDict.TryGetValue(imageData.company_name, out List<ImageData> data_list)) {
                ImageData tempData = data_list[imageData.index];
                tempData.is_display = isDisplay;

                data_list[imageData.index] = tempData;
            }
        }

        public void LoadAllImagesFromFolder() {
            string[] raw_files = GetDirFiles(_rootpath);

            if (raw_files != null && raw_files.Length > 0)
            {
                int l = raw_files.Length;
                for (int i = 0; i < l; i++) {

                    if (IsFormatAllow(raw_files[i])) {
                        ImageData imageData = ParseImageData(raw_files[i]);

                        if (IsImageValid(imageData)) {
                            AddImageDataToDict(imageData);
                        }
                    }
                }
            }
        }

        private void AddImageDataToDict(ImageData imageData) {
            if (!imageDict.ContainsKey(imageData.company_name)) {
                imageDict.Add(imageData.company_name, new List<ImageData>());
            }

            if (imageDict[imageData.company_name].Count(x => x.url == imageData.url) == 0) {
                int imageIndex = imageDict[imageData.company_name].Count;
                imageData.index = imageIndex;

                imageDict[imageData.company_name].Add(imageData);
            }
        }

        private string[] GetDirFiles(string targetFolder) {
            if (Directory.Exists(targetFolder))
            {
                return Directory.GetFiles(targetFolder);
            }
            return new string[0];
        }

        private bool IsFormatAllow(string full_path) {
            int formatIndex = full_path.LastIndexOf(".")+1;
            string format = full_path.Substring(formatIndex);

            if (!string.IsNullOrEmpty(format) && allow_file_format.Contains(format)) {
                return true;
            }

            return false;
        }

        private ImageData ParseImageData(string full_path) {
            int filenameIndex = full_path.LastIndexOf("\\") + 1;

            string filename = full_path.Substring(filenameIndex);

            ImageData imageData = new ImageData();

            if (!string.IsNullOrEmpty(filename))
            {
                string[] split = filename.Split(new string[] { "_" }, System.StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 3) {
                    int formatIndex = split[2].LastIndexOf(".");

                    imageData.url = full_path;
                    imageData.company_name = split[0];
                    imageData.title_name = split[2].Substring(0, formatIndex);
                }
            }

            return imageData;
        }

        public static bool IsImageValid(ImageData imageData) {
            return (!string.IsNullOrEmpty(imageData.company_name) && !string.IsNullOrEmpty(imageData.url) && !string.IsNullOrEmpty(imageData.title_name));
        }

        public struct ImageData {
            public int index;

            public string company_name;
            public string title_name;
            public string url;

            public bool is_picked;
            public bool is_display;
            public bool is_not_exist;
        }


    }
}