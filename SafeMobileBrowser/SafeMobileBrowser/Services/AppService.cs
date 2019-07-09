using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SafeApp;
using SafeApp.Utilities;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppService))]

namespace SafeMobileBrowser.Services
{
    public class AppService
    {
        private static MDataInfo _accessContainerMdinfo;
        private static Session _session;

        public bool IsSessionAvailable => _session != null;

        public bool IsAccessContainerMDataInfoAvailable => !_accessContainerMdinfo.Equals(default(MDataInfo));

        public static void InitialiseSession(Session session)
        {
            try
            {
                _session = session;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async Task<MDataInfo> GetAccessContainerMdataInfoAsync()
        {
            try
            {
                _accessContainerMdinfo = await _session.AccessContainer.GetMDataInfoAsync($"apps/{Constants.AppId}");
            }
            catch (FfiException ex)
            {
                Logger.Error(ex);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
            return _accessContainerMdinfo;
        }
    }
}
