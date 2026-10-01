using System;

namespace BarisCemant.Verimor
{
    /// <summary>Raised for every non-2xx HTTP response.</summary>
    public sealed class VerimorApiException : Exception
    {
        public VerimorApiException(int statusCode, ErrorBodyKind bodyKind, string body, string message)
            : base(message)
        {
            StatusCode = statusCode;
            BodyKind = bodyKind;
            Body = body;
        }

        public int StatusCode { get; }

        public ErrorBodyKind BodyKind { get; }

        /// <summary>The response body as text; empty for binary bodies.</summary>
        public string Body { get; }
    }
}
