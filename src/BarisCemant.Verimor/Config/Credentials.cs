using System;

namespace BarisCemant.Verimor
{
    internal static class Credentials
    {
        /// <summary>Rejects blank values without echoing any secret in the message.</summary>
        public static string Require(string? value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(name + " is required.");
            }

            return value!;
        }
    }
}
