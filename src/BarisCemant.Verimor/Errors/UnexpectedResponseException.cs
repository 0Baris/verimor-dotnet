using System;

namespace BarisCemant.Verimor
{
    /// <summary>Raised when a 2xx response does not have the expected shape.</summary>
    public sealed class UnexpectedResponseException : Exception
    {
        public UnexpectedResponseException(string message)
            : base(message)
        {
        }

        public UnexpectedResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
