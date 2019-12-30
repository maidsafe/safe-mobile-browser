using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SafeApp;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.Helpers
{
    public static class FileHelper
    {
        public static async Task<bool> TransferAssetFilesAndInitLoggingAsync()
        {
            try
            {
                var fileTransferService = DependencyService.Get<IPlatformService>();

                var files = new List<AssetFileTransferModel>
                {
                    new AssetFileTransferModel
                    {
                        FileName = "log.toml",
                        TransferLocation = fileTransferService.ConfigFilesPath
                    }
                };

                await fileTransferService.TransferAssetsAsync(files);
                await Session.SetConfigurationFilePathAsync(fileTransferService.ConfigFilesPath);
                await Session.InitLoggingAsync($"log-{DateTime.Now:MMddyyyy-HHmm}.log");
                Logger.Info("Assets transferred");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Info("Assets transfer failed");
                Logger.Error(ex);
            }
            return false;
        }

        public static async Task<string> ReadAssetFileContentAsync(string fileName)
        {
            using (var stream = await FileSystem.OpenAppPackageFileAsync(fileName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
