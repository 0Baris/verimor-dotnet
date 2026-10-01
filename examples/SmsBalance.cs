using System;
using System.Threading.Tasks;
using BarisCemant.Verimor.Sms;

namespace Examples
{
    public static class SmsBalance
    {
        public static async Task RunAsync(SmsClient sms) => Console.WriteLine(await sms.BalanceAsync());
    }
}
