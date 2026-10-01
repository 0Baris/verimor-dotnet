using System.Collections.Generic;

namespace BarisCemant.Verimor.Switch
{
    /// <summary>Options for <see cref="SwitchClient"/>.</summary>
    public sealed class SwitchClientOptions : ClientOptions
    {
        public string Key { get; set; } = string.Empty;

        internal IReadOnlyDictionary<string, string> ToCredentials()
            => new Dictionary<string, string> { ["key"] = Credentials.Require(Key, nameof(Key)) };
    }
}
