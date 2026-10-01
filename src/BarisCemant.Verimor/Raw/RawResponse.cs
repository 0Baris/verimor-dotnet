using System.Collections.Generic;

namespace BarisCemant.Verimor
{
    /// <summary>The unparsed result of a raw call. Non-2xx responses throw instead.</summary>
    public sealed class RawResponse
    {
        public RawResponse(int statusCode, byte[] content, string body, IReadOnlyDictionary<string, string> headers)
        {
            StatusCode = statusCode;
            Content = content;
            Body = body;
            Headers = headers;
        }

        public int StatusCode { get; }

        public byte[] Content { get; }

        public string Body { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }
    }
}
