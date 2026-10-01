using System.Collections.Generic;

namespace BarisCemant.Verimor.WhatsApp
{
    /// <summary>Options for <see cref="WhatsAppClient"/>.</summary>
    public sealed class WhatsAppClientOptions : ClientOptions
    {
        public string ApiKey { get; set; } = string.Empty;

        internal IReadOnlyDictionary<string, string> ToCredentials()
            => new Dictionary<string, string> { ["x-api-key"] = Credentials.Require(ApiKey, nameof(ApiKey)) };
    }
}
