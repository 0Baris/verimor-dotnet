using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BarisCemant.Verimor.Http
{
    internal static class VerimorJson
    {
        public static readonly JsonSerializerOptions Options = Create();

        public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

        // The generated models read and write their wire names through converters that must be
        // registered on the options (the generator registers them in its own host configuration).
        private static JsonSerializerOptions Create()
        {
            var options = new JsonSerializerOptions();
            foreach (var type in typeof(VerimorJson).Assembly.GetTypes())
            {
                if (type.IsAbstract
                    || !typeof(JsonConverter).IsAssignableFrom(type)
                    || type.Namespace == null
                    || !type.Namespace.EndsWith(".Generated.Model", StringComparison.Ordinal)
                    || type.GetConstructor(Type.EmptyTypes) == null)
                {
                    continue;
                }

                options.Converters.Add((JsonConverter)Activator.CreateInstance(type)!);
            }

            return options;
        }
    }
}
