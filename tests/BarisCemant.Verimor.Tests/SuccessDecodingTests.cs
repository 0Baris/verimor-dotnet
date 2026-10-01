using System.Threading.Tasks;
using BarisCemant.Verimor.Sms;
using BarisCemant.Verimor.Tests.Support;
using BarisCemant.Verimor.WhatsApp;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class SuccessDecodingTests
    {
        [Theory]
        [InlineData("", "application/json")]
        [InlineData("not json", "text/plain")]
        [InlineData("{\"a\":1}", "application/json")]
        [InlineData("null", "application/json")]
        public async Task A_malformed_2xx_list_body_is_rejected(string body, string contentType)
        {
            using var server = LoopbackServer.Respond(200, body, contentType);
            var client = (SmsClient)Target.For("sms").Create(server.Uri);

            await Assert.ThrowsAsync<UnexpectedResponseException>(() => client.StatusByIdAsync(1));
        }

        [Fact]
        public async Task A_valid_202_whatsapp_body_is_decoded()
        {
            const string body = "{\"id\":\"3f2504e0-4f89-41d3-9a0c-0305e82c3301\",\"status\":\"queued\"}";
            using var server = LoopbackServer.Respond(202, body);
            var client = (WhatsAppClient)Target.For("whatsapp").Create(server.Uri);

            var result = await client.SendOtpAsync("905551112233", "otp");

            Assert.Equal("queued", result.Status);
            Assert.Equal("3f2504e0-4f89-41d3-9a0c-0305e82c3301", result.Id.ToString());
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"status\":\"queued\"}")]
        [InlineData("accepted")]
        [InlineData("")]
        public async Task An_invalid_202_whatsapp_body_is_rejected(string body)
        {
            using var server = LoopbackServer.Respond(202, body);
            var client = (WhatsAppClient)Target.For("whatsapp").Create(server.Uri);

            await Assert.ThrowsAsync<UnexpectedResponseException>(() => client.SendOtpAsync("905551112233", "otp"));
        }

        [Fact]
        public async Task A_text_balance_is_returned_verbatim()
        {
            using var server = LoopbackServer.Respond(200, "123.45", "text/plain");
            var client = (SmsClient)Target.For("sms").Create(server.Uri);

            Assert.Equal("123.45", await client.BalanceAsync());
        }
    }
}
