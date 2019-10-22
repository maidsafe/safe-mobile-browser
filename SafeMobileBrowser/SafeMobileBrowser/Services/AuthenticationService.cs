using SafeApp;
#if SAFE_APP_MOCK
using SafeApp.MockAuthBindings;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.UserDialogs;
using SafeApp.Core;
using SafeMobileBrowser.Helpers;
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
                        "Authorisation failed",
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
                        App.AppSession = await Session.AppConnectUnregisteredAsync(Constants.AppId);
                        MessagingCenter.Send(this, MessageCenterConstants.Authenticated, encodedResponse);
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
        }

        public async Task ConnectUsingStoredSerialisedConfiguration(string encodedResponse = Constants.HardCodedAuthResponse)
        {
            try
            {
                App.AppSession = await Session.AppConnectUnregisteredAsync(Constants.AppId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessagingCenter.Send(this, MessageCenterConstants.AuthenticationFailed);
                throw;
            }
        }
#endif
    }
}
