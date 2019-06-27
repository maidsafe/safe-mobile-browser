using SafeApp;
#if SAFE_APP_MOCK
using SafeApp.MockAuthBindings;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using SafeApp.Utilities;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthenticationService))]

namespace SafeMobileBrowser.Services
{
    public class AuthenticationService
    {
#if SAFE_APP_MOCK
        public static Authenticator _authenticator;
        public static async Task RequestMockNetworkAuthenticationAsync()
        {
            var (_, reqMsg) = await Session.EncodeUnregisteredRequestAsync(Constants.AppId);
            var ipcReq = await Authenticator.UnRegisteredDecodeIpcMsgAsync(reqMsg);
            var authIpcReq = ipcReq as UnregisteredIpcReq;
            var resMsg = await Authenticator.EncodeUnregisteredRespAsync(authIpcReq.ReqId, true);
            var ipcResponse = await Session.DecodeIpcMessageAsync(resMsg);
            if (ipcResponse.GetType() == typeof(UnregisteredIpcMsg))
            {
                var authResponse = ipcResponse as UnregisteredIpcMsg;
                App.AppSession = await Session.AppUnregisteredAsync(authResponse.SerialisedCfg);
            }
        }

        public static async Task CreateMockAccount()
        {
            try
            {
                var locator = RandomGenerators.GetRandomString(5);
                var secret = RandomGenerators.GetRandomString(5);
                var invitation = RandomGenerators.GetRandomString(5);
                _authenticator = await Authenticator.CreateAccountAsync(locator, secret, invitation);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }
#else
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

        public static async Task RequestNonMockAuthenticationAsync(bool isUnregistered = false)
        {
            try
            {
                var req = string.Empty;
                if (isUnregistered)
                    (_, req) = await Session.EncodeUnregisteredRequestAsync(Constants.AppId);
                else
                    (_, req) = await GenerateEncodedAuthReqAsync();
                var url = UrlFormat.Format(Constants.AppId, req, true);
                var appLaunched = await DependencyService.Get<INativeUriLauncher>().OpenUri(url);
                if (!appLaunched)
                {
                    await Application.Current.MainPage.DisplayAlert("Authorisation failed", "The SAFE Authenticator app is required to authorise this application", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }

        public async Task ProcessAuthenticationResponseAsync(string url)
        {
            try
            {
                MessagingCenter.Send(this, MessageCenterConstants.ProcessingAuthResponse);
                var encodedResponse = RequestHelpers.GetRequestData(url);
                var decodeResponse = await Session.DecodeIpcMessageAsync(encodedResponse);
                var decodedResponseType = decodeResponse.GetType();
                if (decodedResponseType == typeof(UnregisteredIpcMsg))
                {
                    if (decodeResponse is UnregisteredIpcMsg ipcMsg)
                    {
                        App.AppSession = await Session.AppUnregisteredAsync(ipcMsg.SerialisedCfg);
                        MessagingCenter.Send(this, MessageCenterConstants.Authenticated, encodedResponse);
                    }
                }
                else if (decodedResponseType == typeof(AuthIpcMsg))
                {
                    if (decodeResponse is AuthIpcMsg ipcMsg)
                    {
                        using (UserDialogs.Instance.Loading("Connection to the SAFE Network"))
                        {
                            Session session = await Session.AppRegisteredAsync(Constants.AppId, ipcMsg.AuthGranted);
                            AppService.InitialiseSession(session);
                            BookmarkManager.InitialiseSession(session);
                            var bookmarksMDataInfo = await DependencyService.Get<AppService>().GetAccessContainerMdataInfoAsync();
                            DependencyService.Get<BookmarkManager>().SetMdInfo(bookmarksMDataInfo);
                            await DependencyService.Get<BookmarkManager>().FetchBookmarks();
                        }
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Authentication", $"Request not granted", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert("Error", $"Description: {ex.Message}", "OK");
                MessagingCenter.Send(this, MessageCenterConstants.AuthenticationFailed);
            }
        }

        public async Task ConnectUsingHardcodedResponseAsync()
        {
            await ConnectUsingStoredSerialisedConfiguration(Constants.HardCodedAuthResponse);
        }

        public async Task ConnectUsingStoredSerialisedConfiguration(string encodedResponse)
        {
            try
            {
                if (!string.IsNullOrEmpty(encodedResponse))
                {
                    var decodeResult = await Session.DecodeIpcMessageAsync(encodedResponse);
                    if (decodeResult.GetType() == typeof(UnregisteredIpcMsg))
                    {
                        var ipcMsg = decodeResult as UnregisteredIpcMsg;

                        if (ipcMsg != null)
                        {
                            App.AppSession = await Session.AppUnregisteredAsync(ipcMsg.SerialisedCfg);
                        }
                    }
                }
                else
                {
                    throw new NullReferenceException("Null serialised configuration");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessagingCenter.Send(this, MessageCenterConstants.AuthenticationFailed);
                throw ex;
            }
        }
#endif
    }
}
