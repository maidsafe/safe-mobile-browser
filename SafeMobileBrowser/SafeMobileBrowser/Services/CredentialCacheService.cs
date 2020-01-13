﻿// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

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
