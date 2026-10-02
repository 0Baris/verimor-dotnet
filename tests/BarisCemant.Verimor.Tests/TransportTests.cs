using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BarisCemant.Verimor.Http;
using BarisCemant.Verimor.Tests.Support;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class TransportTests
    {
        [Fact]
        public void Default_timeout_is_thirty_seconds()
            => Assert.Equal(TimeSpan.FromSeconds(30), new ClientOptions().Timeout);

        [Fact]
        public async Task Does_not_dispose_or_mutate_an_injected_http_client()
        {
            using var server = LoopbackServer.Respond(200, "{}");
            var http = new HttpClient { Timeout = TimeSpan.FromSeconds(7) };
            var transport = new VerimorTransport(new ClientOptions { HttpClient = http }, server.Uri);

            await transport.SendAsync(TransportRequest.Get("/v2/balance"), CancellationToken.None);

            Assert.Equal(TimeSpan.FromSeconds(7), http.Timeout);
            Assert.Empty(http.DefaultRequestHeaders);
            using var again = await http.GetAsync(server.Uri); // throws if the SDK disposed it
        }

        [Fact]
        public async Task A_5xx_response_is_sent_exactly_once()
        {
            using var server = LoopbackServer.Respond(503, "unavailable", "text/plain");
            var transport = new VerimorTransport(new ClientOptions(), server.Uri);

            var error = await Assert.ThrowsAsync<VerimorApiException>(() =>
                transport.SendAsync(TransportRequest.Get("/v2/balance"), CancellationToken.None));

            Assert.Equal(503, error.StatusCode);
            Assert.Equal(1, server.RequestCount);
        }

        [Fact]
        public async Task Cancellation_is_native_and_not_retried()
        {
            using var server = LoopbackServer.Respond(200, "{}", delay: TimeSpan.FromSeconds(5));
            var transport = new VerimorTransport(new ClientOptions(), server.Uri);
            using var cts = new CancellationTokenSource();

            // Cancel only after the server holds the request, so the test never races the network.
            var sending = transport.SendAsync(TransportRequest.Get("/v2/balance"), cts.Token);
            await server.FirstRequest.WaitAsync(TimeSpan.FromSeconds(10));
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => sending);
            Assert.Equal(1, server.RequestCount);
        }

        [Fact]
        public async Task Timeout_is_native_and_not_retried()
        {
            using var server = LoopbackServer.Respond(200, "{}", delay: TimeSpan.FromSeconds(8));
            var transport = new VerimorTransport(
                new ClientOptions { Timeout = TimeSpan.FromSeconds(2) }, server.Uri);

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                transport.SendAsync(TransportRequest.Get("/v2/balance"), CancellationToken.None));

            await server.FirstRequest.WaitAsync(TimeSpan.FromSeconds(10));
            Assert.Equal(1, server.RequestCount);
        }

        [Fact]
        public async Task Sends_query_headers_and_json_body()
        {
            using var server = LoopbackServer.Respond(200, "{\"ok\":true}");
            var transport = new VerimorTransport(new ClientOptions(), server.Uri);
            var request = new TransportRequest(
                "POST",
                "/v2/send.json",
                new System.Collections.Generic.Dictionary<string, string> { ["a b"] = "c&d" },
                new System.Collections.Generic.Dictionary<string, string> { ["x-api-key"] = "k" },
                "{\"x\":1}");

            var response = await transport.SendAsync(request, CancellationToken.None);

            Assert.Equal(200, response.StatusCode);
            var seen = Assert.Single(server.Requests);
            Assert.Equal("POST", seen.Method);
            Assert.Equal("/v2/send.json", seen.Path);
            Assert.Equal("?a%20b=c%26d", seen.Query);
            Assert.Equal("k", seen.Headers["x-api-key"]);
            Assert.Equal("{\"x\":1}", seen.Body);
        }
    }
}
