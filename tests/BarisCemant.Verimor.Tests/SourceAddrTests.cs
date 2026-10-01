using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BarisCemant.Verimor.Sms;
using BarisCemant.Verimor.Tests.Support;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class SourceAddrTests
    {
        private static SmsClient Client(LoopbackServer server, string? defaultSender)
            => new SmsClient(new SmsClientOptions
            {
                Username = Target.SmsUser,
                Password = Target.SmsPassword,
                DefaultSender = defaultSender,
                BaseUri = server.Uri,
            });

        [Fact]
        public async Task Send_uses_the_wire_field_names_and_the_client_credentials()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");

            await Client(server, "DEFAULT").SendAsync("905551112233", "hello");

            var seen = Assert.Single(server.Requests);
            Assert.Equal("POST", seen.Method);
            Assert.Equal("/v2/send.json", seen.Path);
            using var body = JsonDocument.Parse(seen.Body);
            var root = body.RootElement;
            Assert.Equal(Target.SmsUser, root.GetProperty("username").GetString());
            Assert.Equal(Target.SmsPassword, root.GetProperty("password").GetString());
            Assert.Equal("DEFAULT", root.GetProperty("source_addr").GetString());
            var message = root.GetProperty("messages").EnumerateArray().Single();
            Assert.Equal("905551112233", message.GetProperty("dest").GetString());
            Assert.Equal("hello", message.GetProperty("msg").GetString());
            Assert.Equal(
                new[] { "messages", "password", "source_addr", "username" },
                root.EnumerateObject().Select(p => p.Name).OrderBy(n => n).ToArray());
        }

        [Fact]
        public async Task Per_call_sender_beats_the_client_default()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");

            await Client(server, "DEFAULT").SendAsync("905551112233", "hello", sourceAddr: "CALL");

            using var body = JsonDocument.Parse(Assert.Single(server.Requests).Body);
            Assert.Equal("CALL", body.RootElement.GetProperty("source_addr").GetString());
        }

        [Fact]
        public async Task Source_addr_is_omitted_when_neither_the_call_nor_the_client_sets_it()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");

            await Client(server, null).SendAsync("905551112233", "hello");

            using var body = JsonDocument.Parse(Assert.Single(server.Requests).Body);
            Assert.False(body.RootElement.TryGetProperty("source_addr", out _));
        }

        [Fact]
        public async Task Legacy_send_puts_the_default_sender_in_the_query()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");

            await Client(server, "DEFAULT").Campaigns.SendLegacyAsync("905551112233", "hello");

            Assert.Contains("source_addr=DEFAULT", Assert.Single(server.Requests).Query);
        }
    }
}
