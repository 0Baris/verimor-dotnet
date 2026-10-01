using System.Collections.Generic;
using System.Text;

namespace BarisCemant.Verimor.Http
{
    internal sealed class TransportResponse
    {
        public TransportResponse(
            int statusCode,
            byte[] content,
            string? contentType,
            IReadOnlyDictionary<string, string> headers)
        {
            StatusCode = statusCode;
            Content = content;
            ContentType = contentType;
            Headers = headers;
        }

        public int StatusCode { get; }

        public byte[] Content { get; }

        public string? ContentType { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }

        public string Body => Encoding.UTF8.GetString(Content);
    }
}
