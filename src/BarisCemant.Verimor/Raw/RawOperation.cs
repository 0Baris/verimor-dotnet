using System.Collections.Generic;

namespace BarisCemant.Verimor
{
    /// <summary>One Verimor operation: its id, HTTP method, path template and credential wiring.</summary>
    public sealed class RawOperation
    {
        public RawOperation(
            string operationId,
            string method,
            string pathTemplate,
            IReadOnlyDictionary<string, string> credentialLocations,
            string? senderLocation = null)
        {
            OperationId = operationId;
            Method = method;
            PathTemplate = pathTemplate;
            CredentialLocations = credentialLocations;
            SenderLocation = senderLocation;
        }

        public string OperationId { get; }

        public string Method { get; }

        public string PathTemplate { get; }

        /// <summary>Credential field name to where it is sent: "query", "header" or "body".</summary>
        public IReadOnlyDictionary<string, string> CredentialLocations { get; }

        /// <summary>Where <c>source_addr</c> goes ("query" or "body"); null when the operation has no sender.</summary>
        public string? SenderLocation { get; }
    }
}
