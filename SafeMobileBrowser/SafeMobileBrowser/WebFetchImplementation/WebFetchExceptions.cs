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
