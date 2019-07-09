using System;

namespace SafeMobileBrowser.Helpers
{
    public static class UrlFormat
    {
        // Add safe-auth:// in encoded auth request
        public static string Format(string appId, string encodedString, bool toAuthenticator)
        {
            string scheme = toAuthenticator ? "safe-auth" : $"{appId}";
            return $"{scheme}://{appId}/{encodedString}";
        }

        // Return encoded response from the url sent from the Authenticator
        public static string GetRequestData(string url)
        {
            return new Uri(url).PathAndQuery.Replace("/", string.Empty);
        }
    }
}
