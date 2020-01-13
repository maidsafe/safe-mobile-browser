// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

namespace SafeMobileBrowser.WebFetchImplementation
{
    public static class WebFetchConstants
    {
        // Type tags and path constants used in web fetch
        public const int DnsTagType = 15001;
        public const int WwwTagType = 15002;
        public const int NfsFileStartIndex = 0;
        public const int NfsFileEndIndex = 0;
        public const string IndexFileName = "index.html";
        public const string DefaultService = "www";

        // Error codes used in web fetch service
        public const int NoSuchPublicName = 1000;
        public const int NoSuchServiceName = 1001;
        public const int NullUrl = 1006;
        public const int ServiceNotFound = 1022;
        public const int NoSuchData = -103;
        public const int NoSuchEntry = -106;
        public const int FileNotFound = -301;

        // Error Messages used in web fetch service
        public const string NoSuchPublicNameMessage = "No such public name exists.";
        public const string NoSuchServiceNameMessage = "No such service name exists.";
        public const string NullUrlMessage = "Provided url is null or empty.";
        public const string ServiceNotFoundMessage = "Required service not found.";
        public const string NoSuchDataMessage = "No such data exists.";
        public const string NoSuchEntryMessage = "No such entry exists.";
        public const string FileNotFoundMessage = "File not found.";
    }
}
