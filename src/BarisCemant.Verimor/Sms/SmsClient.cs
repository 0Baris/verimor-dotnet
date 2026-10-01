using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BarisCemant.Verimor.Sms.Generated.Client;
using BarisCemant.Verimor.Sms.Generated.Model;

namespace BarisCemant.Verimor.Sms
{
    public sealed partial class SmsClient
    {
        /// <summary>Sends one message. Uses <c>DefaultSender</c> unless <paramref name="sourceAddr"/> is given.</summary>
        public Task<string> SendAsync(
            string destination,
            string message,
            string? sourceAddr = null,
            CancellationToken cancellationToken = default)
        {
            var request = new SendSmsJsonRequest(
                string.Empty,
                string.Empty,
                new List<SendSmsJsonRequestMessagesInner> { new SendSmsJsonRequestMessagesInner(destination, message) },
                sourceAddr: sourceAddr == null ? default : new Option<string?>(sourceAddr));
            return Campaigns.SendAsync(request, cancellationToken);
        }

        /// <summary>Sends a prepared request. Pass empty strings for the request's username and password.</summary>
        public Task<string> SendAsync(SendSmsJsonRequest request, CancellationToken cancellationToken = default)
            => Campaigns.SendAsync(request, cancellationToken);

        public Task<string> BalanceAsync(CancellationToken cancellationToken = default)
            => Balances.BalanceAsync(cancellationToken);

        public Task<IReadOnlyList<GetSmsStatus200ResponseInner>> StatusByIdAsync(
            long id, CancellationToken cancellationToken = default)
            => Reports.StatusAsync(id: id, cancellationToken: cancellationToken);

        public Task<IReadOnlyList<GetSmsStatus200ResponseInner>> StatusByCustomIdAsync(
            string customId, CancellationToken cancellationToken = default)
            => Reports.StatusAsync(customId: customId, cancellationToken: cancellationToken);
    }
}
