using System.Collections.Generic;

namespace SafeMobileBrowser.Models
{
    public class WebFetchResponse
    {
        public Dictionary<string, string> Headers { get; set; }

        public string MimeType { get; set; }

        public byte[] Data { get; set; }

        public ulong NrsVersion { get; set; }

        public WebFetchResponse()
        {
            Headers = new Dictionary<string, string>();
        }
    }
}
