using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Utility
{
    public class FileUtility
    {
        private string _rootpath;

        private HashSet<string> files_storage;

        public void SetTargetFolder(string targetFolder) {
            _rootpath = targetFolder;
        }

        private string[] GetDirFiles(string targetFolder) {
            if (Directory.Exists(targetFolder))
            {
                return Directory.GetFiles(targetFolder);
            }
            return new string[0];
        }



    }
}