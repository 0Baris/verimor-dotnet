using System;
using System.Text.Json;

namespace BarisCemant.Verimor.Http
{
    /// <summary>Turns a 2xx response into the operation's declared result, or fails loudly.</summary>
    internal static class ResponseReader
    {
        public static string Text(TransportResponse response) => response.Body;

        public static byte[] Bytes(TransportResponse response) => response.Content;

        public static T Json<T>(TransportResponse response)
        {
            try
            {
                var value = JsonSerializer.Deserialize<T>(response.Body, VerimorJson.Options);
                if (value == null)
                {
                    throw new UnexpectedResponseException("Verimor API returned an empty JSON body.");
                }

                return value;
            }
            catch (UnexpectedResponseException)
            {
                throw;
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                // Generated converters throw ArgumentException (missing required property) as well as JsonException.
                throw new UnexpectedResponseException(
                    "Verimor API returned a body that is not the expected JSON.", exception);
            }
        }

        public static JsonElement Element(TransportResponse response)
        {
            try
            {
                using (var document = JsonDocument.Parse(response.Body))
                {
                    return document.RootElement.Clone();
                }
            }
            catch (JsonException exception)
            {
                throw new UnexpectedResponseException("Verimor API returned a body that is not JSON.", exception);
            }
        }
    }
}
