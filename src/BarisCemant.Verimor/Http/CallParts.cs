using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BarisCemant.Verimor.Http
{
    /// <summary>The caller-supplied pieces of one operation call.</summary>
    internal sealed class CallParts
    {
        public Dictionary<string, string> Path { get; } = new Dictionary<string, string>();

        public Dictionary<string, string> Query { get; } = new Dictionary<string, string>();

        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public Dictionary<string, string>? Form { get; private set; }

        public string? JsonBody { get; set; }

        public void AddPath(string name, object value) => Path[name] = Format(value);

        public void AddQuery(string name, object? value)
        {
            if (value != null)
            {
                Query[name] = Format(value);
            }
        }

        public void AddHeader(string name, object? value)
        {
            if (value != null)
            {
                Headers[name] = Format(value);
            }
        }

        public void AddForm(string name, object? value)
        {
            if (value != null)
            {
                (Form ??= new Dictionary<string, string>())[name] = Format(value);
            }
        }

        private static string Format(object value)
        {
            switch (value)
            {
                case string text:
                    return text;
                case bool flag:
                    return flag ? "true" : "false";
                case IFormattable number:
                    return number.ToString(null, CultureInfo.InvariantCulture);
                case IEnumerable items:
                    return string.Join(",", items.Cast<object>().Select(Format));
                default:
                    return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
            }
        }
    }
}
