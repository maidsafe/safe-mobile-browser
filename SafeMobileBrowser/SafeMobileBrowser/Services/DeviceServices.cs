using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SafeApp;
using SafeMobileBrowser.Models;
using Xamarin.Forms;

namespace SafeMobileBrowser.Services
{
    public static class DeviceServices
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
                Debug.WriteLine("Assets transferred");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Assets transfer failed: " + ex.Message);
            }
        }
    }
}
