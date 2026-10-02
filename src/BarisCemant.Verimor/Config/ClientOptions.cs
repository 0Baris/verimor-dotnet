using System;
using System.Net.Http;

namespace BarisCemant.Verimor
{
    /// <summary>Options shared by every product client.</summary>
    public class ClientOptions
    {
        /// <summary>Server address; defaults to Verimor's address for the product. An IP, a port and a path prefix are kept.</summary>
        public Uri? BaseUri { get; set; }

        /// <summary>Per-request timeout. Defaults to 30 seconds. The SDK never retries.</summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Optional caller-owned client. The SDK never disposes it and never changes its
        /// <see cref="HttpClient.Timeout"/> or default headers.
        /// </summary>
        public HttpClient? HttpClient { get; set; }
    }
}
