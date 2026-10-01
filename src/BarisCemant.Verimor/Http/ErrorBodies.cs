using System;
using System.Text;
using System.Text.Json;

namespace BarisCemant.Verimor.Http
{
    internal static class ErrorBodies
    {
        private const int MaxSummaryLength = 200;
        private static readonly string[] MessageFields = { "message", "detail", "error", "msg" };

        public static ErrorBodyKind Classify(byte[] content, string? contentType)
        {
            if (content.Length == 0)
            {
                return ErrorBodyKind.Empty;
            }

            var type = (contentType ?? string.Empty).ToLowerInvariant();
            if (type.StartsWith("application/octet-stream", StringComparison.Ordinal)
                || type.StartsWith("image/", StringComparison.Ordinal)
                || type.StartsWith("audio/", StringComparison.Ordinal)
                || type.StartsWith("video/", StringComparison.Ordinal)
                || type.StartsWith("application/pdf", StringComparison.Ordinal)
                || type.StartsWith("application/zip", StringComparison.Ordinal))
            {
                return ErrorBodyKind.Binary;
            }

            string text;
            try
            {
                text = new UTF8Encoding(false, true).GetString(content);
            }
            catch (ArgumentException)
            {
                return ErrorBodyKind.Binary;
            }

            return TryParse(text) != null ? ErrorBodyKind.Json : ErrorBodyKind.Text;
        }

        public static VerimorApiException ToException(TransportResponse response)
        {
            var kind = Classify(response.Content, response.ContentType);
            var body = kind == ErrorBodyKind.Binary ? string.Empty : response.Body;
            return new VerimorApiException(
                response.StatusCode, kind, body, BuildMessage(response.StatusCode, kind, body));
        }

        private static string BuildMessage(int status, ErrorBodyKind kind, string body)
        {
            var summary = kind == ErrorBodyKind.Json ? JsonSummary(body) : TextSummary(body);
            return summary.Length == 0
                ? "Verimor API returned HTTP " + status
                : "Verimor API returned HTTP " + status + ": " + summary;
        }

        private static string JsonSummary(string body)
        {
            using (var document = TryParse(body))
            {
                if (document != null && document.RootElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var field in MessageFields)
                    {
                        if (document.RootElement.TryGetProperty(field, out var value)
                            && value.ValueKind == JsonValueKind.String)
                        {
                            return Truncate(value.GetString() ?? string.Empty);
                        }
                    }
                }
            }

            return TextSummary(body);
        }

        private static string TextSummary(string body) => Truncate(body.Trim());

        private static string Truncate(string value)
            => value.Length <= MaxSummaryLength ? value : value.Substring(0, MaxSummaryLength);

        public static JsonDocument? TryParse(string text)
        {
            try
            {
                return JsonDocument.Parse(text);
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
