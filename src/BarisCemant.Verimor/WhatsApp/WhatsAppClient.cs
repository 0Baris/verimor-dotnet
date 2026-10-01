using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BarisCemant.Verimor.WhatsApp.Generated.Client;
using BarisCemant.Verimor.WhatsApp.Generated.Model;

namespace BarisCemant.Verimor.WhatsApp
{
    public sealed partial class WhatsAppClient
    {
        public Task<MessageResponse> SendOtpAsync(
            string to,
            string templateName,
            string? language = null,
            IEnumerable<string>? parameters = null,
            CancellationToken cancellationToken = default)
            => Messages.SendOtpAsync(Template(to, templateName, language, parameters), cancellationToken);

        public Task<MessageResponse> SendUtilityAsync(
            string to,
            string templateName,
            string? language = null,
            IEnumerable<string>? parameters = null,
            CancellationToken cancellationToken = default)
            => Messages.SendUtilityAsync(Template(to, templateName, language, parameters), cancellationToken);

        private static TemplateMessageRequest Template(
            string to, string templateName, string? language, IEnumerable<string>? parameters)
            => new TemplateMessageRequest(
                to,
                templateName,
                language: language == null ? default : new Option<string?>(language),
                parameters: parameters == null ? default : new Option<List<string>?>(parameters.ToList()));
    }
}
