using System;
using SafeApp;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppService))]

namespace SafeMobileBrowser.Services
{
    public class AppService
    {
        private static Session _session;

        public bool IsSessionAvailable => _session != null;

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
    }
}
