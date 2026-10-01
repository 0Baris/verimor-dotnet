using System;
using System.Threading.Tasks;
using BarisCemant.Verimor.Switch;

namespace Examples
{
    public static class SwitchOriginate
    {
        public static async Task RunAsync()
        {
            var calls = new SwitchClient(new SwitchClientOptions
            {
                Key = Environment.GetEnvironmentVariable("VERIMOR_SWITCH_API_KEY") ?? string.Empty,
            });
            Console.WriteLine(await calls.OriginateAsync(extension: "101", destination: "905000000000"));
        }
    }
}
