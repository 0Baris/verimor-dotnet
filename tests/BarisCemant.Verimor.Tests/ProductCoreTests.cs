using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BarisCemant.Verimor.Http;
using BarisCemant.Verimor.Tests.Support;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class ProductCoreTests
    {
        private static readonly IReadOnlyDictionary<string, string> NoCredentials =
            new Dictionary<string, string>();

        private static ProductCore Core(
            LoopbackServer server, Dictionary<string, string>? credentials = null, string? sender = null)
            => new ProductCore(
                new VerimorTransport(new ClientOptions(), server.Uri),
                credentials ?? new Dictionary<string, string>(),
                sender);

        private static RawOperation Operation(
            string method, string path, Dictionary<string, string>? credentials = null, string? sender = null)
            => new RawOperation("op", method, path, credentials ?? new Dictionary<string, string>(), sender);

        [Fact]
        public async Task Query_credentials_are_added_and_never_override_caller_values()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");
            var core = Core(server, new Dictionary<string, string> { ["key"] = "secret" });
            var operation = Operation("GET", "/queues", new Dictionary<string, string> { ["key"] = "query" });

            await core.SendAsync(operation, new CallParts(), CancellationToken.None);
            var withCaller = new CallParts();
            withCaller.AddQuery("key", "caller");
            await core.SendAsync(operation, withCaller, CancellationToken.None);

            Assert.Equal("?key=secret", server.Requests[0].Query);
            Assert.Equal("?key=caller", server.Requests[1].Query);
        }

        [Fact]
        public async Task Header_credentials_use_the_header_name()
        {
            using var server = LoopbackServer.Respond(200, "{}");
            var core = Core(server, new Dictionary<string, string> { ["x-api-key"] = "k1" });
            var operation = Operation("GET", "/health", new Dictionary<string, string> { ["x-api-key"] = "header" });

            await core.SendAsync(operation, new CallParts(), CancellationToken.None);

            Assert.Equal("k1", Assert.Single(server.Requests).Headers["x-api-key"]);
        }

        [Fact]
        public async Task Body_credentials_are_merged_into_the_json_object()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");
            var core = Core(server, new Dictionary<string, string> { ["username"] = "u", ["password"] = "p" });
            var operation = Operation(
                "POST",
                "/v2/cancel/{id}",
                new Dictionary<string, string> { ["username"] = "body", ["password"] = "body" });
            var call = new CallParts { JsonBody = "{\"extra\":1}" };
            call.AddPath("id", 7L);

            await core.SendAsync(operation, call, CancellationToken.None);

            var seen = Assert.Single(server.Requests);
            Assert.Equal("/v2/cancel/7", seen.Path);
            using var body = JsonDocument.Parse(seen.Body);
            Assert.Equal("u", body.RootElement.GetProperty("username").GetString());
            Assert.Equal("p", body.RootElement.GetProperty("password").GetString());
            Assert.Equal(1, body.RootElement.GetProperty("extra").GetInt32());
        }

        [Fact]
        public async Task Body_credentials_fill_empty_placeholders_but_keep_caller_values()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");
            var core = Core(server, new Dictionary<string, string> { ["username"] = "u", ["password"] = "p" });
            var operation = Operation(
                "POST", "/x", new Dictionary<string, string> { ["username"] = "body", ["password"] = "body" });

            await core.SendAsync(
                operation,
                new CallParts { JsonBody = "{\"username\":\"\",\"password\":null}" },
                CancellationToken.None);
            await core.SendAsync(
                operation,
                new CallParts { JsonBody = "{\"username\":\"mine\",\"password\":\"\"}" },
                CancellationToken.None);

            Assert.Equal("{\"username\":\"u\",\"password\":\"p\"}", server.Requests[0].Body);
            Assert.Equal("{\"username\":\"mine\",\"password\":\"p\"}", server.Requests[1].Body);
        }

        [Fact]
        public async Task Body_credentials_create_a_body_when_the_call_has_none()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");
            var core = Core(server, new Dictionary<string, string> { ["username"] = "u" });
            var operation = Operation("POST", "/x", new Dictionary<string, string> { ["username"] = "body" });

            await core.SendAsync(operation, new CallParts(), CancellationToken.None);

            Assert.Equal("{\"username\":\"u\"}", Assert.Single(server.Requests).Body);
        }

        [Fact]
        public async Task Default_sender_is_used_only_when_the_call_has_none()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");
            var core = Core(server, sender: "DEFAULT");
            var operation = Operation("POST", "/v2/send.json", sender: "body");

            await core.SendAsync(operation, new CallParts { JsonBody = "{}" }, CancellationToken.None);
            await core.SendAsync(operation, new CallParts { JsonBody = "{\"source_addr\":\"CALL\"}" }, CancellationToken.None);

            Assert.Equal("{\"source_addr\":\"DEFAULT\"}", server.Requests[0].Body);
            Assert.Equal("{\"source_addr\":\"CALL\"}", server.Requests[1].Body);
        }

        [Fact]
        public async Task Source_addr_is_omitted_when_neither_the_call_nor_the_client_sets_it()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");
            var core = Core(server, sender: null);

            await core.SendAsync(
                Operation("POST", "/v2/send.json", sender: "body"),
                new CallParts { JsonBody = "{\"a\":1}" },
                CancellationToken.None);

            Assert.Equal("{\"a\":1}", Assert.Single(server.Requests).Body);
        }

        [Fact]
        public async Task Default_sender_goes_to_the_query_for_query_operations()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");
            var core = Core(server, sender: "DEFAULT");

            await core.SendAsync(Operation("GET", "/v2/send", sender: "query"), new CallParts(), CancellationToken.None);

            Assert.Equal("?source_addr=DEFAULT", Assert.Single(server.Requests).Query);
        }

        [Fact]
        public async Task Path_values_are_escaped_and_missing_values_are_rejected()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");
            var core = Core(server);
            var operation = Operation("GET", "/fax_document/{id}");
            var call = new CallParts();
            call.AddPath("id", "a b/c");

            await core.SendAsync(operation, call, CancellationToken.None);

            Assert.Equal("/fax_document/a%20b%2Fc", Assert.Single(server.Requests).Path);
            await Assert.ThrowsAsync<ArgumentException>(() =>
                core.SendAsync(operation, new CallParts(), CancellationToken.None));
        }

        [Fact]
        public async Task Form_bodies_are_url_encoded()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");
            var core = Core(server);
            var call = new CallParts();
            call.AddForm("name", "a b");
            call.AddForm("sounddata", "x&y");

            await core.SendAsync(Operation("POST", "/announcements"), call, CancellationToken.None);

            var seen = Assert.Single(server.Requests);
            Assert.Equal("name=a+b&sounddata=x%26y", seen.Body);
            Assert.StartsWith("application/x-www-form-urlencoded", seen.Headers["Content-Type"]);
        }

        [Fact]
        public async Task Two_cores_never_share_credentials()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");
            var first = Core(server, new Dictionary<string, string> { ["key"] = "one" });
            var second = Core(server, new Dictionary<string, string> { ["key"] = "two" });
            var operation = Operation("GET", "/q", new Dictionary<string, string> { ["key"] = "query" });

            await first.SendAsync(operation, new CallParts(), CancellationToken.None);
            await second.SendAsync(operation, new CallParts(), CancellationToken.None);

            Assert.Equal(new[] { "?key=one", "?key=two" }, server.Requests.Select(r => r.Query).ToArray());
        }
    }
}
