using System;
using System.Threading.Tasks;
using BarisCemant.Verimor.Sms;
using BarisCemant.Verimor.Tests.Support;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class ServerUrlTests
    {
        [Fact]
        public async Task A_custom_server_url_keeps_its_path_prefix()
        {
            using var server = LoopbackServer.Respond(200, "1", "text/plain");
            var baseUri = new Uri(server.Uri.ToString().TrimEnd('/') + "/verimor");

            await new SmsClient(new SmsClientOptions { Username = "u", Password = "p", BaseUri = baseUri }).BalanceAsync();

            Assert.Equal("/verimor/v2/balance", server.Requests[0].Path);
        }
    }
}
