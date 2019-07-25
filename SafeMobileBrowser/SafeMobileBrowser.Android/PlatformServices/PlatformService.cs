using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.CustomTabs;
using Plugin.CurrentActivity;
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

        public string BaseUrl => "file:///android_asset";

        public Task<bool> OpenUri(string uri)
        {
            bool result;
            try
            {
                var aUri = Android.Net.Uri.Parse(uri);
                var intent = new Intent(Intent.ActionView, aUri);
                intent.AddFlags(ActivityFlags.ClearTask);
                intent.AddFlags(ActivityFlags.NewTask);
                CrossCurrentActivity.Current.AppContext.StartActivity(intent);
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

        public void LaunchNativeEmbeddedBrowser(string url)
        {
            var activity = CrossCurrentActivity.Current.Activity;

            if (activity == null)
            {
                return;
            }

            var customTabsActivityManager = new CustomTabsActivityManager(activity);

            customTabsActivityManager.CustomTabsServiceConnected += (name, client) =>
            {
                customTabsActivityManager.LaunchUrl(url);
            };

            if (!customTabsActivityManager.BindService())
            {
                var uri = Android.Net.Uri.Parse(url);
                var intent = new Intent(Intent.ActionView, uri);
                activity.StartActivity(intent);
            }
        }
    }
}
