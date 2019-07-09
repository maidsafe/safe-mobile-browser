using System;
using System.Threading.Tasks;
using SafeMobileBrowser.Helpers;
using Xamarin.Essentials;

namespace SafeMobileBrowser.Services
{
    internal static class CredentialCacheService
    {
        private const string AuthRspKey = "UnregisteredAuthResponse";

        public static void Delete()
        {
            try
            {
                SecureStorage.RemoveAll();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static async Task<string> Retrieve()
        {
            return await SecureStorage.GetAsync(AuthRspKey);
        }

        public static async Task Store(string authRsp)
        {
            await SecureStorage.SetAsync(AuthRspKey, authRsp);
        }
    }
}
