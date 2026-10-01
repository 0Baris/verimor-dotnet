using System.Threading;
using System.Threading.Tasks;
using BarisCemant.Verimor.Switch.Generated.Client;
using BarisCemant.Verimor.Switch.Generated.Model;

namespace BarisCemant.Verimor.Switch
{
    public sealed partial class SwitchClient
    {
        /// <summary>Starts a call from <paramref name="extension"/> to <paramref name="destination"/>.</summary>
        public Task<string> OriginateAsync(
            string extension,
            string destination,
            string? callerId = null,
            CancellationToken cancellationToken = default)
        {
            var request = new OriginateCallPostRequest(
                extension,
                destination,
                callerId: callerId == null ? default : new Option<string?>(callerId));
            return Calls.OriginateAsync(request, cancellationToken);
        }
    }
}
