using System.Collections.Generic;

namespace BarisCemant.Verimor
{
    /// <summary>A request for <see cref="RawClient"/>: path values, query, headers and an optional JSON body.</summary>
    public sealed class RawRequest
    {
        public Dictionary<string, string> PathValues { get; } = new Dictionary<string, string>();

        public Dictionary<string, string> Query { get; } = new Dictionary<string, string>();

        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public string? JsonBody { get; set; }
    }
}
