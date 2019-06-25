using System.Threading.Tasks;

namespace SafeMobileBrowser.Services
{
    public interface INativeUriLauncher
    {
        Task<bool> OpenUri(string uri);
    }
}
