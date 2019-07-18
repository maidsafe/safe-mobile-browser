using System;
using System.Collections.Generic;
using System.Text;

namespace SafeMobileBrowser.Controls
{
    public interface INativeBrowserService
    {
        void LaunchNativeEmbeddedBrowser(string url);
    }
}
