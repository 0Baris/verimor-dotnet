using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BarisCemant.Verimor.Tests.Support;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class OperationMatrixTests
    {
        public static IEnumerable<object[]> Operations()
            => Contract.Load().Select(o => new object[] { o.Product, o.OperationId, o.Proxy, o.Method, o.Path });

        [Fact]
        public void The_contract_lists_all_72_operations()
        {
            var operations = Contract.Load();
            Assert.Equal(72, operations.Count);
            Assert.Equal(14, operations.Count(o => o.Product == "sms"));
            Assert.Equal(52, operations.Count(o => o.Product == "switch"));
            Assert.Equal(6, operations.Count(o => o.Product == "whatsapp"));
            Assert.Equal(72, operations.Select(o => (o.Product, o.Proxy)).Distinct().Count());
        }

        [Theory]
        [MemberData(nameof(Operations))]
        public async Task Every_operation_reaches_its_declared_endpoint_with_its_credentials(
            string product, string operationId, string proxy, string method, string path)
        {
            var target = Target.For(product);
            var parts = proxy.Split('.');
            var serviceProperty = target.ClientType.GetProperty(parts[0])!;
            var methodInfo = serviceProperty.PropertyType.GetMethod(parts[1])!;
            Type? result = methodInfo.ReturnType.IsGenericType ? methodInfo.ReturnType.GetGenericArguments()[0] : null;

            using var server = Samples.Server(result);
            var client = target.Create(server.Uri);
            var service = serviceProperty.GetValue(client)!;
            var arguments = methodInfo.GetParameters()
                .Select(p => p.ParameterType == typeof(CancellationToken)
                    ? (object)CancellationToken.None
                    : Samples.Create(p.ParameterType))
                .ToArray();

            await (Task)methodInfo.Invoke(service, arguments)!;

            var seen = Assert.Single(server.Requests);
            Assert.Equal(method, seen.Method);
            Assert.Matches("^" + Regex.Replace(Regex.Escape(path), @"\\\{[^}]+\}|\{[^}]+\}", "[^/]+") + "$", seen.Path);

            var raw = ((RawClient)target.ClientType.GetProperty("Raw")!.GetValue(client)!)
                .Operations.Single(o => o.OperationId == operationId);
            Assert.Equal(method, raw.Method);
            Assert.Equal(path, raw.PathTemplate);
            foreach (var credential in raw.CredentialLocations)
            {
                switch (credential.Value)
                {
                    case "query":
                        Assert.Contains(credential.Key + "=", seen.Query);
                        break;
                    case "header":
                        Assert.True(seen.Headers.ContainsKey(credential.Key));
                        break;
                    case "body":
                        using (var body = JsonDocument.Parse(seen.Body))
                        {
                            Assert.False(string.IsNullOrEmpty(body.RootElement.GetProperty(credential.Key).GetString()));
                        }

                        break;
                }
            }
        }

        [Fact]
        public void Credential_wiring_matches_the_contract_counts()
        {
            int Count(string product, string location)
            {
                var target = Target.For(product);
                var client = target.Create(new Uri("http://127.0.0.1:1/"));
                var raw = (RawClient)target.ClientType.GetProperty("Raw")!.GetValue(client)!;
                return raw.Operations.Count(o => o.CredentialLocations.Values.Contains(location));
            }

            Assert.Equal(10, Count("sms", "query"));
            Assert.Equal(4, Count("sms", "body"));
            Assert.Equal(50, Count("switch", "query"));
            Assert.Equal(5, Count("whatsapp", "header"));
        }
    }
}
