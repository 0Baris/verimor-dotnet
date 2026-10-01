using System;
using System.Threading.Tasks;
using BarisCemant.Verimor.Sms;

namespace Examples
{
    public static class SendSms
    {
        public static async Task RunAsync()
        {
            var sms = new SmsClient(new SmsClientOptions
            {
                Username = Environment.GetEnvironmentVariable("VERIMOR_SMS_USERNAME") ?? string.Empty,
                Password = Environment.GetEnvironmentVariable("VERIMOR_SMS_PASSWORD") ?? string.Empty,
                DefaultSender = "VERIMOR",
            });
            var campaignId = await sms.SendAsync("905000000000", "Merhaba");
            Console.WriteLine(campaignId);
        }
    }
}
