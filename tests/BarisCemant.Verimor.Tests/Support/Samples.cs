using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using BarisCemant.Verimor.Http;
using BarisCemant.Verimor.Sms;
using BarisCemant.Verimor.Switch;
using BarisCemant.Verimor.WhatsApp;

namespace BarisCemant.Verimor.Tests.Support
{
    /// <summary>A product's client type and a factory that points a client at a loopback server.</summary>
    internal sealed class Target
    {
        public const string SmsUser = "sms-user";
        public const string SmsPassword = "sms-password";
        public const string SwitchKey = "switch-key";
        public const string WhatsAppKey = "whatsapp-key";
        public const string DefaultSender = "DEFSENDER";

        private Target(Type clientType, Func<Uri, object> create)
        {
            ClientType = clientType;
            Create = create;
        }

        public Type ClientType { get; }

        public Func<Uri, object> Create { get; }

        public static Target For(string product)
        {
            switch (product)
            {
                case "sms":
                    return new Target(typeof(SmsClient), uri => new SmsClient(new SmsClientOptions
                    {
                        Username = SmsUser, Password = SmsPassword, DefaultSender = DefaultSender, BaseUri = uri,
                    }));
                case "switch":
                    return new Target(typeof(SwitchClient), uri => new SwitchClient(new SwitchClientOptions
                    {
                        Key = SwitchKey, BaseUri = uri,
                    }));
                case "whatsapp":
                    return new Target(typeof(WhatsAppClient), uri => new WhatsAppClient(new WhatsAppClientOptions
                    {
                        ApiKey = WhatsAppKey, BaseUri = uri,
                    }));
                default:
                    throw new ArgumentOutOfRangeException(nameof(product));
            }
        }
    }

    /// <summary>Builds argument values and matching response bodies for any generated type by reflection.</summary>
    internal static class Samples
    {
        public static object Create(Type type)
        {
            var underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null)
            {
                return Create(underlying);
            }

            if (type == typeof(string)) return "x";
            if (type == typeof(long)) return 1L;
            if (type == typeof(int)) return 1;
            if (type == typeof(bool)) return true;
            if (type == typeof(double)) return 1.0;
            if (type == typeof(float)) return 1.0f;
            if (type == typeof(Guid)) return Guid.NewGuid();
            if (type == typeof(DateTime)) return DateTime.UtcNow;
            if (type == typeof(DateTimeOffset)) return DateTimeOffset.UtcNow;
            if (type == typeof(object)) return "x";
            if (type.IsEnum) return Enum.GetValues(type).GetValue(0)!;
            if (type.IsGenericType)
            {
                var definition = type.GetGenericTypeDefinition();
                var item = type.GetGenericArguments()[0];
                if (definition == typeof(IEnumerable<>) || definition == typeof(IReadOnlyList<>)
                    || definition == typeof(List<>) || definition == typeof(IList<>))
                {
                    var list = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(item))!;
                    list.Add(Create(item));
                    return list;
                }

                if (definition == typeof(Dictionary<,>))
                {
                    return Activator.CreateInstance(type)!;
                }
            }

            var constructor = type.GetConstructors()
                .OrderByDescending(c => c.GetCustomAttribute<JsonConstructorAttribute>() != null)
                .ThenByDescending(c => c.GetParameters().Length)
                .First();
            var arguments = constructor.GetParameters()
                .Select(p => IsOption(p.ParameterType) ? Activator.CreateInstance(p.ParameterType)! : Create(p.ParameterType))
                .ToArray();
            return constructor.Invoke(arguments);
        }

        public static LoopbackServer Server(Type? result)
        {
            if (result == null) return LoopbackServer.Respond(200, "");
            if (result == typeof(string)) return LoopbackServer.Respond(200, "ok", "text/plain");
            if (result == typeof(byte[])) return LoopbackServer.RespondBytes(200, new byte[] { 1, 2, 3 }, "application/pdf");
            if (result == typeof(JsonElement)) return LoopbackServer.Respond(200, "{}");
            return LoopbackServer.Respond(200, JsonSerializer.Serialize(Create(result), result, VerimorJson.Options));
        }

        private static bool IsOption(Type type)
            => type.IsGenericType && type.GetGenericTypeDefinition().Name.StartsWith("Option`", StringComparison.Ordinal);
    }
}
