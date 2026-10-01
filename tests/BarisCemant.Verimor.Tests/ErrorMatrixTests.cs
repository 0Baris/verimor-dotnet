using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarisCemant.Verimor.Sms;
using BarisCemant.Verimor.Switch;
using BarisCemant.Verimor.Tests.Support;
using BarisCemant.Verimor.WhatsApp;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class ErrorMatrixTests
    {
        private static readonly int[] Statuses = { 400, 401, 403, 404, 422, 500, 503 };
        private static readonly string[] Products = { "sms", "switch", "whatsapp" };

        public static IEnumerable<object[]> Cases()
            => from product in Products
               from status in Statuses
               from kind in Enum.GetValues(typeof(ErrorBodyKind)).Cast<ErrorBodyKind>()
               select new object[] { product, status, kind };

        [Fact]
        public void The_matrix_has_84_cases() => Assert.Equal(84, Cases().Count());

        [Theory]
        [MemberData(nameof(Cases))]
        public async Task Every_non_2xx_response_becomes_a_verimor_api_exception(
            string product, int status, ErrorBodyKind kind)
        {
            using var server = Respond(status, kind);
            var call = Call(product, server.Uri);

            var error = await Assert.ThrowsAsync<VerimorApiException>(call);

            Assert.Equal(status, error.StatusCode);
            Assert.Equal(kind, error.BodyKind);
            Assert.False(string.IsNullOrWhiteSpace(error.Message));
            Assert.Contains(status.ToString(), error.Message);
            foreach (var secret in new[] { Target.SmsUser, Target.SmsPassword, Target.SwitchKey, Target.WhatsAppKey })
            {
                Assert.DoesNotContain(secret, error.Message);
            }

            Assert.Equal(1, server.RequestCount);
        }

        [Fact]
        public async Task A_json_error_message_field_is_surfaced()
        {
            using var server = LoopbackServer.Respond(422, "{\"message\":\"invalid destination\"}");
            var error = await Assert.ThrowsAsync<VerimorApiException>(Call("sms", server.Uri));
            Assert.Contains("invalid destination", error.Message);
        }

        private static LoopbackServer Respond(int status, ErrorBodyKind kind)
        {
            switch (kind)
            {
                case ErrorBodyKind.Json:
                    return LoopbackServer.Respond(status, "{\"message\":\"rejected\"}");
                case ErrorBodyKind.Text:
                    return LoopbackServer.Respond(status, "rejected", "text/plain");
                case ErrorBodyKind.Empty:
                    return LoopbackServer.Respond(status, "", "text/plain");
                default:
                    return LoopbackServer.RespondBytes(status, new byte[] { 0x00, 0xFF, 0x10 }, "application/octet-stream");
            }
        }

        private static Func<Task> Call(string product, Uri uri)
        {
            switch (product)
            {
                case "sms":
                    var sms = (SmsClient)Target.For(product).Create(uri);
                    return () => sms.BalanceAsync();
                case "switch":
                    var calls = (SwitchClient)Target.For(product).Create(uri);
                    return () => calls.Calls.HangupAsync("1");
                default:
                    var whatsApp = (WhatsAppClient)Target.For(product).Create(uri);
                    return () => whatsApp.Health.HealthAsync();
            }
        }
    }
}
