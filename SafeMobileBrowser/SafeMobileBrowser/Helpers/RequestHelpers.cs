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
        // Add safe-auth:// in encoded auth request
        public static string UrlFormat(string encodedString, bool toAuthenticator)
        {
            var scheme = toAuthenticator ? "safe-auth" : $"{Constants.AppId}";
            return $"{scheme}://{encodedString}";
        }

        public static string GetRequestData(string url)
        {
            return new Uri(url).PathAndQuery.Replace("/", string.Empty);
        }

        // Generating encoded app request using appname, appid, vendor
        public static async Task<(uint, string)> GenerateEncodedAppRequestAsync()
        {
            Console.WriteLine("Generating application authentication request");
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
            List<ByteRange> byteRanges = new List<ByteRange>();

            var bytes = "bytes=";
            var rangeValues = rangeString.Remove(0, bytes.Length).Split(',');

            foreach (var item in rangeValues)
            {
                var part = item.Split('-');
                var rangeitem = new ByteRange();

                if (part.Length == 2)
                {
                    if (part[1] == string.Empty)
                    {
                        rangeitem.End = 0;
                    }
                    else
                    {
                        rangeitem.End = (ulong)Convert.ToInt32(part[1]);
                    }
                }
                rangeitem.Start = (ulong)Convert.ToInt32(part[0]);

                byteRanges.Add(rangeitem);
            }

            return byteRanges;
        }
    }
}
