using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SafeApp;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

namespace SafeMobileBrowser.Helpers
{
    public static class FileTransferHelper
    {
        public static async Task TransferAssetFiles()
        {
            try
            {
                var files = new List<AssetFileTransferModel>
                {
                    new AssetFileTransferModel()
                    {
                        FileName = "log.toml",
                        TransferLocation = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                    }
                };

                var fileTransferService = DependencyService.Get<IPlatformService>();
                await fileTransferService.TransferAssetsAsync(files);
                await Session.SetAdditionalSearchPathAsync(fileTransferService.ConfigFilesPath);
                await Session.InitLoggingAsync();
                Logger.Info("Assets transferred");
            }
            catch (Exception ex)
            {
                Logger.Info("Assets transfer failed");
                Logger.Error(ex);
            }
        }
    }
}
