using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BarisCemant.Verimor.Http;

namespace BarisCemant.Verimor
{
    /// <summary>Complete access to every operation of one product by operation id.</summary>
    public sealed class RawClient
    {
        private readonly ProductCore _core;
        private readonly Dictionary<string, RawOperation> _byId;

        internal RawClient(ProductCore core, IEnumerable<RawOperation> operations)
        {
            _core = core;
            Operations = operations.ToList();
            _byId = Operations.ToDictionary(operation => operation.OperationId, StringComparer.Ordinal);
        }

        public IReadOnlyList<RawOperation> Operations { get; }

        public async Task<RawResponse> SendAsync(
            string operationId, RawRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!_byId.TryGetValue(operationId, out var operation))
            {
                throw new ArgumentException("Unknown operation id: " + operationId, nameof(operationId));
            }

            var call = new CallParts { JsonBody = request.JsonBody };
            foreach (var pair in request.PathValues)
            {
                call.AddPath(pair.Key, pair.Value);
            }

            foreach (var pair in request.Query)
            {
                call.AddQuery(pair.Key, pair.Value);
            }

            foreach (var pair in request.Headers)
            {
                call.AddHeader(pair.Key, pair.Value);
            }

            var response = await _core.SendAsync(operation, call, cancellationToken).ConfigureAwait(false);
            return new RawResponse(response.StatusCode, response.Content, response.Body, response.Headers);
        }
    }
}
