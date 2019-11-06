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
        private HashSet<string> allow_file_format = new HashSet<string> { "png", "jpg", "jpeg" };

        //Company name, Image url list
        private Dictionary<string, List<ImageData>> imageDict = new Dictionary<string, List<ImageData>>();

        public int CompanyCount {
            get {
                return imageDict.Keys.Count;
            }
        }

        public string GetCompanykeyByIndex(int keyIndex)
        {
            if (keyIndex < 0 || keyIndex > imageDict.Keys.Count)
                return null;

            return imageDict.Keys.ElementAt(keyIndex);
        }

        public void SetTargetFolder(string targetFolder) {
            imageDict.Clear();
            _rootpath = targetFolder;

            LoadAllImagesFromFolder();
        }

        public async void RefreshImageRecordDisplay() {

            if (imageDict == null) return;

            await Task.Run(() =>
            {
                foreach (KeyValuePair<string, List<ImageData>> CompanyItem in imageDict) {
                    int imageCount = CompanyItem.Value.Count;
                    for (int i = 0; i < imageCount; i++)
                    {
                        MarkImageVisibility(CompanyItem.Value[i], false);
                    }
                }
            });
        }

        public ImageData GetRandomImage(bool recordDisplay, int draw_time = 0) {
            int keyCount = imageDict.Count;
            int random_companyIndex = Random.Range(0, keyCount);
            int maxDrawTime = 10;
            //Debug.Log(random_companyIndex);

            string key = imageDict.Keys.ElementAt(random_companyIndex);

            if (imageDict.TryGetValue(key, out List<ImageData> rawImageDataList)) {

                var filterImageDataList = rawImageDataList.FindAll(x => !x.is_display);
                int rawImageCount = rawImageDataList.Count;
                int filterImageCount = filterImageDataList.Count;

                int image_index = Random.Range(0, filterImageCount);
                if (image_index <= 0 && draw_time < maxDrawTime) {
                    draw_time += 1;
                    return GetRandomImage(recordDisplay, draw_time);
                }

                if (filterImageCount > 0) {
                    ImageData imageData = filterImageDataList.ElementAt(image_index);

                    if (recordDisplay)
                        MarkImageVisibility(imageData, true);

                    return imageData;
                }

                if (rawImageCount > 0)
                {
                    int raw_image_index = Random.Range(0, rawImageCount);
                    ImageData imageData = rawImageDataList.ElementAt(raw_image_index);

                    return imageData;
                }
            }

            return default(ImageData);
        }

        public ImageData GetRandomImageByTag(string company, bool notPickBefore)
        {

            if (!string.IsNullOrEmpty(company) && imageDict.TryGetValue(company, out List<ImageData> imageData))
            {
                if (notPickBefore)
                    imageData = imageData.FindAll(x => !x.is_picked);

                int imageDataCount = imageData.Count;
                int image_index = Random.Range(0, imageDataCount);

                if (image_index >= 0 && imageDataCount > 0)
                    return imageData.ElementAt(image_index);
            }

            return default(ImageData);
        }

        public void MarkPrizeDrawPrivilege(ImageData imageData, bool is_picked)
        {
            if (imageDict.TryGetValue(imageData.company_name, out List<ImageData> data_list))
            {
                ImageData tempData = data_list[imageData.index];
                tempData.is_picked = is_picked;

                data_list[imageData.index] = tempData;
            }
        }

        public void MarkImageVisibility(ImageData imageData, bool isDisplay) {

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

            if (imageDict[imageData.company_name].Count(x => x.url == imageData.url) == 0)
            {
                int imageIndex = imageDict[imageData.company_name].Count;
                imageData.index = imageIndex;

                imageDict[imageData.company_name].Add(imageData);
            }
            else {
                MarkImageVisibility(imageData , false);
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

                if (split.Length == 4) {
                    int formatIndex = split[3].LastIndexOf(".");

                    imageData.url = full_path;
                    imageData.company_name = split[0];
                    imageData.title_name = split[3].Substring(0, formatIndex);
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