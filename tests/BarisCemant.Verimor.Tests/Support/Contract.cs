using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BarisCemant.Verimor.Tests.Support
{
    internal sealed class ContractOperation
    {
        public string Product { get; set; } = "";
        public string OperationId { get; set; } = "";
        public string Method { get; set; } = "";
        public string Path { get; set; } = "";
        public string Proxy { get; set; } = "";
    }

    internal static class Contract
    {
        public static IReadOnlyList<ContractOperation> Load()
        {
            var file = Path.Combine(AppContext.BaseDirectory, "contracts", "operations.json");
            using (var document = JsonDocument.Parse(File.ReadAllText(file)))
            {
                return document.RootElement.EnumerateArray()
                    .Select(item => new ContractOperation
                    {
                        Product = item.GetProperty("product").GetString()!,
                        OperationId = item.GetProperty("operationId").GetString()!,
                        Method = item.GetProperty("method").GetString()!,
                        Path = item.GetProperty("path").GetString()!,
                        Proxy = item.GetProperty("proxy").GetString()!,
                    })
                    .ToList();
            }
        }
    }
}
