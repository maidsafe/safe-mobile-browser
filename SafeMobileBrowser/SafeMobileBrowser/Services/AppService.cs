using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SafeApp;
using SafeApp.Utilities;
using SafeMobileBrowser.Helpers;

namespace SafeMobileBrowser.Services
{
    public class AppService
    {
        private static MDataInfo mdinfo;
        private static Session _session;

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

        public async Task<MDataInfo> GetMdInfoAsync()
        {
            try
            {
                mdinfo = await _session.AccessContainer.GetMDataInfoAsync($"apps/{Constants.AppId}");
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
            return mdinfo;
        }

        public Session FetchSession()
        {
            return _session;
        }
    }
}
