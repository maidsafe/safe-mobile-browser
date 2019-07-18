using System.Collections.Generic;
using System.Threading.Tasks;
using SafeMobileBrowser.Models;

namespace SafeMobileBrowser.Services
{
    public interface IPlatformService
    {
        string ConfigFilesPath { get; }

        string BaseUrl { get; }

        Task TransferAssetsAsync(List<AssetFileTransferModel> fileList);

        Task<bool> OpenUri(string uri);

        void LaunchNativeEmbeddedBrowser(string url);
    }
}
