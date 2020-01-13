// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using Acr.UserDialogs;
using Android.Views;
using Android.Webkit;

namespace SafeMobileBrowser.Droid.MediaDownload
{
    public class ImageDownloadMenuItemListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
    {
        private readonly string _itemExtra;

        public ImageDownloadMenuItemListener(string extra)
        {
            _itemExtra = extra;
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            if (string.IsNullOrEmpty(_itemExtra))
                return false;

            var fileName = _itemExtra.StartsWith("data:image") ?
                            $"image.{DataImage.GetImageExtension(_itemExtra)}" :
                            URLUtil.GuessFileName(_itemExtra, null, null);

            if (FileHelper.MediaExists(fileName))
            {
                UserDialogs.Instance.ActionSheet(new ActionSheetConfig()
                .SetTitle("An image with the same name already exists")
                .SetMessage($"Do you want replace the existing {fileName} in download")
                .Add(
                    "Replace file",
                    () => new ImageDownloader().Execute(_itemExtra, fileName))
                .Add(
                    "Create new file",
                    () => new ImageDownloader().Execute(_itemExtra, FileHelper.GenerateNewFileName(fileName)))
                .SetCancel()
                .SetUseBottomSheet(true));
            }
            else
            {
                new ImageDownloader().Execute(_itemExtra, fileName);
            }

            return true;
        }
    }
}
