using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using SafeMobileBrowser.Droid.PlatformServices;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(PlatformService))]

namespace SafeMobileBrowser.Droid.PlatformServices
{
    public class PlatformService : IPlatformService
    {
        public string ConfigFilesPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public string BaseUrl => "file:///android_asset/";

        public Task<bool> OpenUri(string uri)
        {
            bool result;
            try
            {
                var aUri = Android.Net.Uri.Parse(uri.ToString());
                var intent = new Intent(Intent.ActionView, aUri);
#pragma warning disable CS0618 // Type or member is obsolete
                Forms.Context.StartActivity(intent);
#pragma warning restore CS0618 // Type or member is obsolete
                result = true;
            }
            catch (ActivityNotFoundException)
            {
                result = false;
            }

            return Task.FromResult(result);
        }

        public async Task TransferAssetsAsync(List<AssetFileTransferModel> fileList)
        {
            foreach (var file in fileList)
            {
                using (var reader = new StreamReader(Application.Context.Assets.Open(file.FileName)))
                {
                    using (var writer = new StreamWriter(Path.Combine(file.TransferLocation, file.FileName)))
                    {
                        await writer.WriteAsync(await reader.ReadToEndAsync());
                        writer.Close();
                    }
                    reader.Close();
                }
            }
        }
    }
}
