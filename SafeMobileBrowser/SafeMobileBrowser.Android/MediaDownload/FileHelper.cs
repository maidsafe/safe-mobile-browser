using System;
using System.IO;
using System.Linq;
using Android.Graphics;
using Plugin.CurrentActivity;
using SafeMobileBrowser.Helpers;
using AEnvironment = Android.OS.Environment;
using Path = System.IO.Path;

namespace SafeMobileBrowser.Droid.MediaDownload
{
    public static class FileHelper
    {
        private static string DownloadPath => Path.Combine(
            CrossCurrentActivity.Current.Activity.ApplicationContext.GetExternalFilesDir(null).AbsolutePath,
            AEnvironment.DirectoryDownloads);

        public static void ExportBitmapAsFile(Bitmap image, string fileType, string fileName)
        {
            try
            {
                var filePath = Path.Combine(DownloadPath, fileName);
                SaveMedia(image, filePath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static bool MediaExists(string fileName)
        {
            var filePath = Path.Combine(DownloadPath, fileName);
            return File.Exists(filePath);
        }

        public static void SaveImageAtDownloads(byte[] byteImageData, string fileName)
        {
            try
            {
                var filePath = Path.Combine(DownloadPath, fileName);
                File.WriteAllBytes(filePath, byteImageData.Concat(new[] { (byte)0xD9 }).ToArray());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static void SaveMedia(Bitmap image, string filePath)
        {
            try
            {
                var stream = new FileStream(filePath, FileMode.Create);
                image.Compress(Bitmap.CompressFormat.Png, 100, stream);
                stream.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static string GenerateNewFileName(string oldFileName)
        {
            int num = 0;
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(oldFileName);
            var fileExtension = Path.GetExtension(oldFileName);
            string newFileName;
            do
            {
                num++;
                newFileName = $"{fileNameWithoutExtension}-{num}{fileExtension}";
            }
            while (MediaExists(newFileName));
            return newFileName;
        }
    }
}
