using System;
using System.Linq;
using System.Threading.Tasks;
using Examples.Operations;

namespace Examples
{
    // dotnet run --project examples -- sms/send
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            if (args.Length != 1 || !Catalog.All.TryGetValue(args[0], out var run))
            {
                Console.Error.WriteLine("usage: dotnet run --project examples -- <product>/<operation>");
                Console.Error.WriteLine(string.Join(Environment.NewLine, Catalog.All.Keys.OrderBy(key => key)));
                return 2;
            }

            try
            {
                await run();
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                return 1;
            }
        }
    }
}
