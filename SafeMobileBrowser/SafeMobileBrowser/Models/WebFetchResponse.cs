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

namespace SafeMobileBrowser.Models
{
    public class WebFetchResponse
    {
        public Dictionary<string, string> Headers { get; set; }

        public string MimeType { get; set; }

        public byte[] Data { get; set; }

        public ulong CurrentNrsVersion { get; set; }

        public ulong LatestNrsVersion { get; set; }

        public Type FetchDataType { get; set; }

        public WebFetchResponse()
        {
            Headers = new Dictionary<string, string>();
        }
    }
}
