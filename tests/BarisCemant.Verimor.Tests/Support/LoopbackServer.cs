using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarisCemant.Verimor.Tests.Support
{
    internal sealed class RecordedRequest
    {
        public string Method { get; set; } = "";
        public string Path { get; set; } = "";
        public string Query { get; set; } = "";
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public string Body { get; set; } = "";
    }

    /// <summary>A loopback-only HTTP server so no test can reach a real endpoint.</summary>
    internal sealed class LoopbackServer : IDisposable
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly List<RecordedRequest> _requests = new List<RecordedRequest>();
        private readonly object _gate = new object();
        private readonly int _status;
        private readonly byte[] _body;
        private readonly string _contentType;
        private readonly TimeSpan _delay;

        private LoopbackServer(int status, byte[] body, string contentType, TimeSpan delay)
        {
            _status = status;
            _body = body;
            _contentType = contentType;
            _delay = delay;
            var probe = new TcpListener(IPAddress.Loopback, 0);
            probe.Start();
            var port = ((IPEndPoint)probe.LocalEndpoint).Port;
            probe.Stop();
            Uri = new Uri("http://127.0.0.1:" + port + "/");
            _listener.Prefixes.Add(Uri.ToString());
            _listener.Start();
            _ = Task.Run(LoopAsync);
        }

        public Uri Uri { get; }

        public int RequestCount
        {
            get { lock (_gate) { return _requests.Count; } }
        }

        public IReadOnlyList<RecordedRequest> Requests
        {
            get { lock (_gate) { return _requests.ToArray(); } }
        }

        public static LoopbackServer Respond(
            int status, string body, string contentType = "application/json", TimeSpan? delay = null)
            => new LoopbackServer(status, Encoding.UTF8.GetBytes(body), contentType, delay ?? TimeSpan.Zero);

        public static LoopbackServer RespondBytes(int status, byte[] body, string contentType)
            => new LoopbackServer(status, body, contentType, TimeSpan.Zero);

        public void Dispose() => _listener.Close();

        private async Task LoopAsync()
        {
            while (_listener.IsListening)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    return;
                }

                _ = Task.Run(() => HandleAsync(context));
            }
        }

        private async Task HandleAsync(HttpListenerContext context)
        {
            try
            {
                var recorded = new RecordedRequest
                {
                    Method = context.Request.HttpMethod,
                    Path = context.Request.Url!.AbsolutePath,
                    Query = context.Request.Url.Query,
                };
                foreach (var name in context.Request.Headers.AllKeys)
                {
                    recorded.Headers[name!] = context.Request.Headers[name]!;
                }

                using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                {
                    recorded.Body = await reader.ReadToEndAsync().ConfigureAwait(false);
                }

                lock (_gate) { _requests.Add(recorded); }
                if (_delay > TimeSpan.Zero)
                {
                    await Task.Delay(_delay).ConfigureAwait(false);
                }

                context.Response.StatusCode = _status;
                context.Response.ContentType = _contentType;
                context.Response.ContentLength64 = _body.Length;
                await context.Response.OutputStream.WriteAsync(_body, 0, _body.Length).ConfigureAwait(false);
                context.Response.Close();
            }
            catch (Exception)
            {
                // The client may have gone away (cancellation and timeout tests).
            }
        }
    }
}
