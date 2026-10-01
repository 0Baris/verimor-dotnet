using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BarisCemant.Verimor.Http;
using BarisCemant.Verimor.Tests.Support;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class RawClientTests
    {
        private static RawOperation Op(string id, string method, string path)
            => new RawOperation(id, method, path, new Dictionary<string, string>());

        [Fact]
        public async Task Sends_any_listed_operation_by_id()
        {
            using var server = LoopbackServer.Respond(200, "{\"ok\":true}");
            var core = new ProductCore(
                new VerimorTransport(new ClientOptions(), server.Uri), new Dictionary<string, string>(), null);
            var raw = new RawClient(core, new[] { Op("get_thing", "GET", "/things/{id}") });
            var request = new RawRequest();
            request.PathValues["id"] = "9";
            request.Query["page"] = "2";

            var response = await raw.SendAsync("get_thing", request, CancellationToken.None);

            Assert.Equal(200, response.StatusCode);
            Assert.Equal("{\"ok\":true}", response.Body);
            var seen = Assert.Single(server.Requests);
            Assert.Equal("/things/9", seen.Path);
            Assert.Equal("?page=2", seen.Query);
            Assert.Equal("get_thing", raw.Operations.Single().OperationId);
        }

        [Fact]
        public async Task Unknown_operation_ids_are_rejected_without_a_request()
        {
            using var server = LoopbackServer.Respond(200, "{}");
            var core = new ProductCore(
                new VerimorTransport(new ClientOptions(), server.Uri), new Dictionary<string, string>(), null);
            var raw = new RawClient(core, new[] { Op("a", "GET", "/a") });

            await Assert.ThrowsAsync<System.ArgumentException>(() =>
                raw.SendAsync("missing", new RawRequest(), CancellationToken.None));
            Assert.Equal(0, server.RequestCount);
        }
    }
}
