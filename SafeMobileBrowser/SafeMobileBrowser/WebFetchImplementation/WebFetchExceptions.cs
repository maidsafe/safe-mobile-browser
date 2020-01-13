// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Runtime.Serialization;

namespace SafeMobileBrowser.WebFetchImplementation
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class WebFetchException : Exception
    {
        public int ErrorCode { get; set; }

        public WebFetchException()
        {
        }

        public WebFetchException(string message)
            : base(message)
        {
        }

        public WebFetchException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public WebFetchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public WebFetchException(int errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
}
