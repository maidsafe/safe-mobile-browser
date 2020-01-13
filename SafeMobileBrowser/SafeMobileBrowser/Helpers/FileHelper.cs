// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

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
