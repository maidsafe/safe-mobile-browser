﻿// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

namespace SafeMobileBrowser.Helpers
{
    public static class Constants
    {
        public const string AppName = "Safe Browser";
        public const string AppId = "net.maidsafe.sn_browser";
        public const string Vendor = "MaidSafe.net Ltd";
        public const string AppStateMdEntryKey = "safeBrowserState";

        public const string ConnectingProgressText = "Connecting to the Safe Network";
        public const string BookmarkRemovedSuccessfully = "Bookmark removed successfully";
        public const string BookmarkAddedSuccessfully = "Bookmark added successfully";
        public const string LogFileContentReadSuccessfully = "Log file content copied to clipboard";
        public const string LogFileDeleteSuccessfully = "Log file deleted";
        public const string DeleteLogFilesAlertTitle = "Delete log files";
        public const string DeleteLogFilesAlertMsg = "This will delete all the log files except the lastest one " +
                                                     "which is currently used by the app";

        public const string CurrentLogFile = "Current log file cannot be deleted";
        public const string S3FileDownloadDialogMsg = "Downloading MaidSafe hosted network info";
        public const string AuthStatusMsg = "Press continue to reset the current session and re-authenticate to start a new session.";
        public const string AuthenticationDialogTitle = "Authentication";
        public const string UseAuthenticator = "Authenticate";
        public const string AuthenticationDialogMsg = "You can connect to a network of your choice using the Authenticator app, or continue to connect to the MaidSafe hosted network.";
        public const string Continue = "Continue";

        // URL
        public const string PrivacyInfoUrl = "https://safenetwork.tech/privacy/";
        public const string FaqUrl = "https://safenetforum.org/t/faqs-safe-browser-mobile/29365";
        public const string AppThemePreferenceKey = "AppThemePreferenceKey";
        public const string AppUpdatePreferenceKey = "AppUpdatePreferenceKey";
        public const string BufferText = "buffer-text";
    }
}
