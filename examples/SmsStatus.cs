using System;
using System.Threading.Tasks;
using BarisCemant.Verimor.Sms;

namespace Examples
{
    public static class SmsStatus
    {
        public static async Task RunAsync(SmsClient sms)
        {
            foreach (var status in await sms.StatusByCustomIdAsync("order-42"))
            {
                Console.WriteLine(status);
            }
        }
    }
}
