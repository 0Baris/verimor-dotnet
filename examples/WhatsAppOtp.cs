using System;
using System.Threading.Tasks;
using BarisCemant.Verimor.WhatsApp;

namespace Examples
{
    public static class WhatsAppOtp
    {
        public static async Task RunAsync()
        {
            var whatsApp = new WhatsAppClient(new WhatsAppClientOptions
            {
                ApiKey = Environment.GetEnvironmentVariable("VERIMOR_WHATSAPP_API_KEY") ?? string.Empty,
            });
            var accepted = await whatsApp.SendOtpAsync("905000000000", "otp_template", "tr", new[] { "123456" });
            Console.WriteLine($"{accepted.Id} {accepted.Status}");
        }
    }
}
