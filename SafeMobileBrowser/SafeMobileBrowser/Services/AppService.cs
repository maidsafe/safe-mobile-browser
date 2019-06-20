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
        private static MDataInfo _accesscontainerMdinfo;
        private static Session _session;

        public Session Session
        {
            get => _session;

            private set
            {
                _session = value;
            }
        }

        public bool IsSessionAvailable => _session == null ? false : true;

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
                _accesscontainerMdinfo = await _session.AccessContainer.GetMDataInfoAsync($"apps/{Constants.AppId}");
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
            return _accesscontainerMdinfo;
        }
    }
}
