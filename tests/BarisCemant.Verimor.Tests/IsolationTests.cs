using System;
using System.Linq;
using System.Threading.Tasks;
using BarisCemant.Verimor.Sms;
using BarisCemant.Verimor.Switch;
using BarisCemant.Verimor.Tests.Support;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class IsolationTests
    {
        [Fact]
        public async Task Two_sms_clients_in_one_process_send_their_own_credentials()
        {
            using var server = LoopbackServer.Respond(200, "1", "text/plain");
            var first = new SmsClient(new SmsClientOptions { Username = "u1", Password = "p1", BaseUri = server.Uri });
            var second = new SmsClient(new SmsClientOptions { Username = "u2", Password = "p2", BaseUri = server.Uri });

            await first.BalanceAsync();
            await second.BalanceAsync();
            await first.BalanceAsync();

            var queries = server.Requests.Select(r => r.Query).ToArray();
            Assert.Contains("username=u1", queries[0]);
            Assert.Contains("username=u2", queries[1]);
            Assert.Contains("username=u1", queries[2]);
            Assert.DoesNotContain("u2", queries[2]);
        }

        [Fact]
        public async Task A_switch_client_never_sends_sms_credentials()
        {
            using var server = LoopbackServer.Respond(200, "ok", "text/plain");
            _ = new SmsClient(new SmsClientOptions { Username = "sms-only", Password = "sms-only", BaseUri = server.Uri });
            var calls = new SwitchClient(new SwitchClientOptions { Key = "k", BaseUri = server.Uri });

            await calls.Calls.HangupAsync("1");

            var seen = Assert.Single(server.Requests);
            Assert.DoesNotContain("sms-only", seen.Query + seen.Body + string.Join(",", seen.Headers.Values));
        }

        [Theory]
        [InlineData("", "secret-password", "Username is required.")]
        [InlineData("secret-user", "", "Password is required.")]
        [InlineData(" ", "secret-password", "Username is required.")]
        public void Blank_credentials_are_rejected_without_echoing_secrets(
            string username, string password, string expected)
        {
            var error = Assert.Throws<ArgumentException>(() =>
                new SmsClient(new SmsClientOptions { Username = username, Password = password }));
            Assert.Equal(expected, error.Message);
        }

        [Fact]
        public void Non_positive_timeouts_are_rejected()
            => Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SwitchClient(new SwitchClientOptions { Key = "k", Timeout = TimeSpan.Zero }));
    }
}
