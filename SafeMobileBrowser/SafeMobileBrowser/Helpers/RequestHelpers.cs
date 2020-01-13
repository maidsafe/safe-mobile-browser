// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SafeApp;
using SafeApp.Core;
using SafeMobileBrowser.Models;

namespace SafeMobileBrowser.Helpers
{
    public static class RequestHelpers
    {
        private const string Bytes = "bytes=";

        // Generating encoded app request using app name, appid, vendor
        public static async Task<(uint, string)> GenerateEncodedAppRequestAsync()
        {
            Logger.Info("Generating application authentication request");
            var authReq = new AuthReq
            {
                AppContainer = true,
                App = new AppExchangeInfo
                {
                    Id = Constants.AppId,
                    Scope = string.Empty,
                    Name = Constants.AppName,
                    Vendor = Constants.Vendor
                },
                Containers = new List<ContainerPermissions>()
            };

            return await Session.EncodeAuthReqAsync(authReq);
        }

        public static List<ByteRange> RangeStringToArray(string rangeString)
        {
            var byteRanges = new List<ByteRange>();
            var rangeValues = rangeString.Remove(0, Bytes.Length).Split(',');

            foreach (var item in rangeValues)
            {
                var part = item.Split('-');
                var byteRange = new ByteRange();

                if (part.Length == 2)
                {
                    if (part[1] == string.Empty)
                    {
                        byteRange.End = 0;
                    }
                    else
                    {
                        byteRange.End = (ulong)Convert.ToInt32(part[1]);
                    }
                }
                byteRange.Start = (ulong)Convert.ToInt32(part[0]);

                byteRanges.Add(byteRange);
            }

            return byteRanges;
        }
    }
}
