using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SafeApp;
using SafeApp.Utilities;
using SafeMobileBrowser.Models;

namespace SafeMobileBrowser.Helpers
{
    public static class RequestHelpers
    {
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

            var bytes = "bytes=";
            var rangeValues = rangeString.Remove(0, bytes.Length).Split(',');

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
