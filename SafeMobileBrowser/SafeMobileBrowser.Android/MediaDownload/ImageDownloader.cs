// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
#if __ANDROID_29__
using AndroidX.Core.App;
#else
using Android.Support.V4.App;
#endif
using Android.Webkit;
using Android.Widget;
using Java.IO;
using Nito.AsyncEx.Synchronous;
using Plugin.CurrentActivity;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.WebFetchImplementation;
using Xamarin.Forms;
using AEnvironment = Android.OS.Environment;
using Object = Java.Lang.Object;
using Path = System.IO.Path;

namespace SafeMobileBrowser.Droid.MediaDownload
{
    public class ImageDownloader : AsyncTask
    {
        private const string ChannelName = "browser_channel";
        private const string ChannelId = "browser_notification_channel";
        private const string ChannelDescription = "Notification channel used by browser app";
        private const NotificationImportance ChannelNotificationImportance = NotificationImportance.Low;
        private int _notificationId;
        private string _imageDownloadData;
        private NotificationManager _notificationManager;
        private NotificationCompat.Builder _builder;
        private string _fileName;
        private Random _rand = new Random();

        protected override void OnPreExecute()
        {
            base.OnPreExecute();

            var randNo = _rand.Next(10000000);
            _notificationId = randNo;

            if (_notificationManager == null)
            {
                _notificationManager =
                    (NotificationManager)CrossCurrentActivity.Current.AppContext.GetSystemService(
                        Context.NotificationService);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var mChannel = new NotificationChannel(ChannelId, ChannelName, ChannelNotificationImportance)
                    {
                        Description = ChannelDescription
                    };
                    mChannel.EnableLights(false);
                    mChannel.SetShowBadge(false);
                    _notificationManager.CreateNotificationChannel(mChannel);
                }
            }

            _builder = new NotificationCompat.Builder(CrossCurrentActivity.Current.AppContext, ChannelId);
            _builder.SetContentTitle("Downloading file");
            _builder.SetContentText("Download in progress...");
            _builder.SetSmallIcon(Resource.Drawable.notification_icon);
            _builder.SetAutoCancel(true);
            _notificationManager.Notify(_notificationId, _builder.Build());
        }

        protected override Object DoInBackground(params Object[] @params)
        {
            _imageDownloadData = @params[0].ToString();
            _fileName = @params[1].ToString();

            try
            {
                if (_imageDownloadData.StartsWith("data:image"))
                {
                    var image = DataImage.TryParse(_imageDownloadData);
                    if (image == null)
                        return false;
                    FileHelper.SaveImageAtDownloads(image.RawData, _fileName);
                    return true;
                }

                if (!_imageDownloadData.StartsWith("https"))
                    return false;

                DownloadMedia();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        private void DownloadMedia()
        {
            try
            {
                var task = WebFetchService.FetchResourceAsync(_imageDownloadData);
                var webFetchResponse = task.WaitAndUnwrapException();

                var bitmap = BitmapFactory.DecodeByteArray(
                    webFetchResponse.Data,
                    0,
                    webFetchResponse.Data.Length);

                if (bitmap == null)
                {
                    // work around as some jpg images are encoded incorrectly
                    FileHelper.SaveImageAtDownloads(webFetchResponse.Data, _fileName);
                }
                else
                {
                    FileHelper.ExportBitmapAsFile(bitmap, webFetchResponse.MimeType, _fileName);
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
                throw;
            }
        }

        protected override void OnPostExecute(Object result)
        {
            base.OnPostExecute(result);

            if ((bool)result)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(
                        CrossCurrentActivity.Current.AppContext,
                        "Image downloaded",
                        ToastLength.Long).Show();
                });

                var downloadPath = Path.Combine(
                    CrossCurrentActivity.Current.Activity.ApplicationContext.GetExternalFilesDir(null).AbsolutePath,
                    AEnvironment.DirectoryDownloads);
                var filePath = Path.Combine(downloadPath, _fileName);
                var file = new File(filePath);

                var fileExtension = MimeTypeMap.GetFileExtensionFromUrl(filePath);
                var mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(fileExtension.ToLower());

                var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(filePath));
                intent.SetDataAndType(Android.Net.Uri.FromFile(file), mimeType);
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);
                var pendingIntent = PendingIntent.GetActivity(CrossCurrentActivity.Current.Activity, 0, intent, 0);

                _builder.SetContentTitle("Download completed");
                _builder.SetContentText(_fileName);
                _builder.SetContentIntent(pendingIntent);
                _notificationManager.Notify(_notificationId, _builder.Build());
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(
                        CrossCurrentActivity.Current.AppContext,
                        "Failed to download image",
                        ToastLength.Long).Show();
                });
                _builder.SetContentText("Download failed");
                _notificationManager.Notify(_notificationId, _builder.Build());
            }
        }
    }
}
