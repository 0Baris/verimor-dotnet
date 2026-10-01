using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarisCemant.Verimor.Http
{
    /// <summary>Sends one request, once. There is deliberately no retry logic.</summary>
    internal sealed class VerimorTransport
    {
        private readonly HttpClient _http;
        private readonly Uri _baseUri;
        private readonly TimeSpan _timeout;

        public VerimorTransport(ClientOptions options, Uri defaultBaseUri)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(options), "Timeout must be positive.");
            }

            _timeout = options.Timeout;
            _baseUri = WithTrailingSlash(options.BaseUri ?? defaultBaseUri);
            _http = options.HttpClient ?? new HttpClient { Timeout = System.Threading.Timeout.InfiniteTimeSpan };
        }

        public async Task<TransportResponse> SendAsync(
            TransportRequest request, CancellationToken cancellationToken)
        {
            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                linked.CancelAfter(_timeout);
                using (var message = BuildMessage(request))
                using (var response = await _http.SendAsync(message, linked.Token).ConfigureAwait(false))
                {
                    var content = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var header in response.Headers.Concat(response.Content.Headers))
                    {
                        headers[header.Key] = string.Join(", ", header.Value);
                    }

                    var result = new TransportResponse(
                        (int)response.StatusCode,
                        content,
                        response.Content.Headers.ContentType?.ToString(),
                        headers);
                    if (result.StatusCode < 200 || result.StatusCode > 299)
                    {
                        throw ErrorBodies.ToException(result);
                    }

                    return result;
                }
            }
        }

        private HttpRequestMessage BuildMessage(TransportRequest request)
        {
            var url = new StringBuilder(new Uri(_baseUri, request.Path.TrimStart('/')).ToString());
            var separator = '?';
            foreach (var pair in request.Query.OrderBy(item => item.Key, StringComparer.Ordinal))
            {
                url.Append(separator)
                    .Append(Uri.EscapeDataString(pair.Key))
                    .Append('=')
                    .Append(Uri.EscapeDataString(pair.Value));
                separator = '&';
            }

            var message = new HttpRequestMessage(new HttpMethod(request.Method), url.ToString());
            foreach (var header in request.Headers)
            {
                message.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (request.FormBody != null)
            {
                message.Content = new FormUrlEncodedContent(request.FormBody);
            }
            else if (request.JsonBody != null)
            {
                message.Content = new StringContent(request.JsonBody, Encoding.UTF8, "application/json");
            }

            return message;
        }

        private static Uri WithTrailingSlash(Uri uri)
            => uri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal) ? uri : new Uri(uri.AbsoluteUri + "/");
    }
}
