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
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Acr.UserDialogs;
using SafeApp;
using SafeApp.Core;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthenticationService))]

namespace SafeMobileBrowser.Services
{
    public class AuthenticationService
    {
        private static readonly string _vaultS3DownloadLink = "https://safe-vault-config.s3.eu-west-2.amazonaws.com/shared-section/vault_connection_info.config";

        private static string ConfigFilePath => DependencyService.Get<IPlatformService>().ConfigFilesPath;

        private static readonly string _defaultVaultConnectionFileName = "vault_connection_info.config";

        public static async Task<(uint, string)> GenerateEncodedAuthReqAsync()
        {
            // Create an AuthReq
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

            // Return encoded AuthReq
            return await Session.EncodeAuthReqAsync(authReq);
        }

        public async Task RequestAuthenticationAsync(bool isUnregistered = false)
        {
            try
            {
                App.PendingRequest = true;

                var filePath = Path.Combine(ConfigFilePath, _defaultVaultConnectionFileName);
                if (File.Exists(filePath))
                    File.Delete(filePath);

                string req;
                if (isUnregistered)
                    (_, req) = await Session.EncodeUnregisteredRequestAsync(Constants.AppId);
                else
                    (_, req) = await GenerateEncodedAuthReqAsync();
                var url = UrlFormat.Format(Constants.AppId, req, true);
                var appLaunched = await DependencyService.Get<IPlatformService>().OpenUri(url);
                if (!appLaunched)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Authentication failed",
                        "The SAFE Authenticator app is required to authorise this application",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public async Task ProcessAuthenticationResponseAsync(string url)
        {
            try
            {
                MessagingCenter.Send(this, MessageCenterConstants.ProcessingAuthResponse);
                var encodedResponse = UrlFormat.GetRequestData(url);
                var decodeResponse = await Session.DecodeIpcMessageAsync(encodedResponse);
                var decodedResponseType = decodeResponse.GetType();
                if (decodedResponseType == typeof(UnregisteredIpcMsg))
                {
                    if (decodeResponse is UnregisteredIpcMsg ipcMsg)
                    {
                        using (UserDialogs.Instance.Loading(Constants.ConnectingProgressText))
                        {
                            App.AppSession = await Session.AppConnectAsync(Constants.AppId, encodedResponse);
                            MessagingCenter.Send(this, MessageCenterConstants.Authenticated);
                        }
                    }
                }
                else if (decodedResponseType == typeof(AuthIpcMsg))
                {
                    if (decodeResponse is AuthIpcMsg ipcMsg)
                    {
                        using (UserDialogs.Instance.Loading(Constants.ConnectingProgressText))
                        {
                            Session session = await Session.AppConnectAsync(Constants.AppId, encodedResponse);
                            AppService.InitialiseSession(session);
                        }
                    }
                }
            }
            catch (FfiException ex)
            {
                Logger.Error(ex);
                if (ex.ErrorCode != -106)
                {
                    if (ex.Message.Contains("AuthDenied"))
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            ErrorConstants.AuthenticationFailedTitle,
                            ErrorConstants.RequestDeniedMsg,
                            "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            ErrorConstants.AuthenticationFailedTitle,
                            ErrorConstants.AuthenticationFailedMsg,
                            "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                await Application.Current.MainPage.DisplayAlert("Authentication", "Authentication Failed", "OK");
            }
            finally
            {
                App.PendingRequest = false;
            }
        }

        public async Task ConnectUsingStoredSerialisedConfiguration(string encodedResponse)
        {
            try
            {
                App.AppSession = await Session.AppConnectAsync(Constants.AppId, encodedResponse);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessagingCenter.Send(this, MessageCenterConstants.AuthenticationFailed);
                throw;
            }
        }

        internal async Task DownloadMaidSafeSharedSectionVault()
        {
            try
            {
                using (UserDialogs.Instance.Loading("Connect to the MaidSafe shared section"))
                {
                    using (var client = new HttpClient())
                    {
                        using (var response = await client.GetAsync(_vaultS3DownloadLink))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var fileContent = await response.Content.ReadAsStringAsync();
                                if (fileContent.Length > 0)
                                {
                                    File.WriteAllText(Path.Combine(ConfigFilePath, _defaultVaultConnectionFileName), fileContent);
                                    await Session.SetAppConfigurationDirectoryPathAsync(ConfigFilePath);
                                    App.AppSession = await Session.AppConnectUnregisteredAsync(Constants.AppId);
                                    MessagingCenter.Send(this, MessageCenterConstants.Authenticated);
                                }
                            }
                            else
                            {
                                throw new Exception("Failed to download file from S3.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Download vault connection file",
                    "Failed to download the vault connection file.",
                    "ok");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
