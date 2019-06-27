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

        public Session Session => _session;

        public bool IsSessionAvailable => _session == null ? false : true;

        public bool IsAccessContainerMDataInfoAvailable => _accessContainerMdinfo.Equals(default(MDataInfo)) ? false : true;

        public static void InitialiseSession(Session session)
        {
            try
            {
                _session = session;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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
                Debug.WriteLine("Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error : " + ex.Message);
                throw;
            }
            return _accessContainerMdinfo;
        }
    }
}
