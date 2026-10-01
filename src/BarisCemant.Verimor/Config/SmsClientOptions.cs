using System.Collections.Generic;

namespace BarisCemant.Verimor.Sms
{
    /// <summary>Options for <see cref="SmsClient"/>.</summary>
    public sealed class SmsClientOptions : ClientOptions
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        /// <summary>The sender header (<c>source_addr</c>) used when a call does not set one.</summary>
        public string? DefaultSender { get; set; }

        internal IReadOnlyDictionary<string, string> ToCredentials()
            => new Dictionary<string, string>
            {
                ["username"] = Credentials.Require(Username, nameof(Username)),
                ["password"] = Credentials.Require(Password, nameof(Password)),
            };
    }
}
