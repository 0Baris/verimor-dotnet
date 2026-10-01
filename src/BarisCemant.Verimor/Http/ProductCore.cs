using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace BarisCemant.Verimor.Http
{
    /// <summary>
    /// Everything product-specific that happens before a request is sent: path expansion,
    /// credential placement, and the default SMS sender. Holds this client's credentials only.
    /// </summary>
    internal sealed class ProductCore
    {
        private const string SenderField = "source_addr";

        private readonly VerimorTransport _transport;
        private readonly IReadOnlyDictionary<string, string> _credentials;
        private readonly string? _defaultSender;

        public ProductCore(
            VerimorTransport transport, IReadOnlyDictionary<string, string> credentials, string? defaultSender)
        {
            _transport = transport;
            _credentials = credentials;
            _defaultSender = defaultSender;
        }

        public Task<TransportResponse> SendAsync(
            RawOperation operation, CallParts call, CancellationToken cancellationToken)
        {
            var query = new Dictionary<string, string>(call.Query);
            var headers = new Dictionary<string, string>(call.Headers);
            JsonObject? body = null;

            foreach (var credential in operation.CredentialLocations)
            {
                if (!_credentials.TryGetValue(credential.Key, out var value))
                {
                    continue;
                }

                switch (credential.Value)
                {
                    case "query":
                        AddIfAbsent(query, credential.Key, value);
                        break;
                    case "header":
                        AddIfAbsent(headers, credential.Key, value);
                        break;
                    case "body":
                        body = Body(body, call);
                        if (IsMissingOrEmpty(body, credential.Key))
                        {
                            body[credential.Key] = value;
                        }

                        break;
                }
            }

            if (_defaultSender != null && operation.SenderLocation != null)
            {
                if (operation.SenderLocation == "query")
                {
                    AddIfAbsent(query, SenderField, _defaultSender);
                }
                else
                {
                    body = Body(body, call);
                    if (!body.ContainsKey(SenderField))
                    {
                        body[SenderField] = _defaultSender;
                    }
                }
            }

            var request = new TransportRequest(
                operation.Method,
                ExpandPath(operation.PathTemplate, call.Path),
                query,
                headers,
                body != null ? body.ToJsonString() : call.JsonBody,
                call.Form);
            return _transport.SendAsync(request, cancellationToken);
        }

        private static JsonObject Body(JsonObject? current, CallParts call)
        {
            if (current != null)
            {
                return current;
            }

            if (call.JsonBody == null)
            {
                return new JsonObject();
            }

            return JsonNode.Parse(call.JsonBody) as JsonObject
                ?? throw new ArgumentException("The request body must be a JSON object.");
        }

        // Generated request models require username/password; callers pass "" and the client fills them in.
        private static bool IsMissingOrEmpty(JsonObject body, string key)
        {
            if (!body.TryGetPropertyValue(key, out var node) || node == null)
            {
                return true;
            }

            return node is JsonValue value && value.TryGetValue<string>(out var text) && text.Length == 0;
        }

        private static void AddIfAbsent(Dictionary<string, string> target, string key, string value)
        {
            if (!target.ContainsKey(key))
            {
                target[key] = value;
            }
        }

        private static string ExpandPath(string template, IReadOnlyDictionary<string, string> values)
        {
            var result = template;
            foreach (var pair in values)
            {
                result = result.Replace("{" + pair.Key + "}", Uri.EscapeDataString(pair.Value));
            }

            if (result.IndexOf('{') >= 0)
            {
                throw new ArgumentException("A path value is missing for " + template + ".");
            }

            return result;
        }
    }
}
