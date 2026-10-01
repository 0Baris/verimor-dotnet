using System;
using System.Threading.Tasks;
using BarisCemant.Verimor;
using BarisCemant.Verimor.Switch;

namespace Examples
{
    public static class RawRequestExample
    {
        public static async Task RunAsync(SwitchClient calls)
        {
            var request = new RawRequest();
            request.Query["page"] = "1";
            var response = await calls.Raw.SendAsync("getCdrs", request);
            Console.WriteLine(response.Body);
        }
    }
}
