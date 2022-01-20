using System;
using System.Collections.Generic;
using System.IO;

namespace ExifPhotos
{
    internal class Util
    {
        /// <summary>
        /// get all the photo files under folder and sub folders. returns true if any found
        /// </summary>
        public static bool FindPhotosUnderFolder(string path, IList<string> fileList, bool subFolder)
        {
            int count = fileList.Count;
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists)
            {
                // get photo files
                IEnumerable<string> files = FindPhotosInFolder(path);
                foreach (string file in files) fileList.Add(file);
                // go through sub folders
                if (subFolder)
                {
                    string[] subFolders = Directory.GetDirectories(path);
                    if (subFolders != null && subFolders.Length > 0)
                    {
                        foreach (string item in subFolders) FindPhotosUnderFolder(item, fileList, subFolder);
                    }
                }
            }
            return fileList.Count > count;
        }

        /// <summary>
        /// get all the photo files in the folder NOT sub folders
        /// </summary>
        public static IEnumerable<string> FindPhotosInFolder(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists)
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in Directory.GetFiles(path))
                {
                    if (file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return file;
                    }
                }
            }
        }

        public static bool GetSubFolders(string folderPath, IList<string> folderList)
        {
            DirectoryInfo info = new DirectoryInfo(folderPath);
            if (!info.Exists) return false;

            // get all the sub-folders
            if (folderList != null)
            {
                string[] subFolders = Directory.GetDirectories(folderPath);
                if (subFolders != null && subFolders.Length > 0)
                {
                    foreach (string subFolder in subFolders) folderList.Add(subFolder);
                }
            }
            return true;
        }

        public static bool GetFilesAndSubFolders(string folderPath, IList<string> fileList, IList<string> folderList)
        {
            DirectoryInfo info = new DirectoryInfo(folderPath);
            if (!info.Exists) return false;

            // get all the files in this folder
            if (fileList != null)
            {
                string[] files = Directory.GetFiles(folderPath);
                if (files != null && files.Length > 0)
                {
                    foreach (string file in files) fileList.Add(file);
                }
            }
            // get all the sub-folders
            if (folderList != null)
            {
                string[] subFolders = Directory.GetDirectories(folderPath);
                if (subFolders != null && subFolders.Length > 0)
                {
                    foreach (string subFolder in subFolders) folderList.Add(subFolder);
                }
            }
            return true;
        }

    }
}
