using System.Collections.Generic;

namespace BarisCemant.Verimor.Http
{
    internal sealed class TransportRequest
    {
        private static readonly IReadOnlyDictionary<string, string> NoValues =
            new Dictionary<string, string>();

        public TransportRequest(
            string method,
            string path,
            IReadOnlyDictionary<string, string>? query = null,
            IReadOnlyDictionary<string, string>? headers = null,
            string? jsonBody = null,
            IReadOnlyDictionary<string, string>? formBody = null)
        {
            Method = method;
            Path = path;
            Query = query ?? NoValues;
            Headers = headers ?? NoValues;
            JsonBody = jsonBody;
            FormBody = formBody;
        }

        public string Method { get; }

        public string Path { get; }

        public IReadOnlyDictionary<string, string> Query { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }

        public string? JsonBody { get; }

        public IReadOnlyDictionary<string, string>? FormBody { get; }

        public static TransportRequest Get(string path) => new TransportRequest("GET", path);
    }
}
