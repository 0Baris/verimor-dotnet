using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BarisCemant.Verimor.Sms;
using BarisCemant.Verimor.Switch;
using BarisCemant.Verimor.WhatsApp;

// Installs nothing but the packed BarisCemant.Verimor package and calls each product on loopback.
var probe = new TcpListener(IPAddress.Loopback, 0);
probe.Start();
var port = ((IPEndPoint)probe.LocalEndpoint).Port;
probe.Stop();
var baseUri = new Uri($"http://127.0.0.1:{port}/");
var seen = new ConcurrentQueue<string>();
using var listener = new HttpListener();
listener.Prefixes.Add(baseUri.ToString());
listener.Start();
_ = Task.Run(async () =>
{
    while (listener.IsListening)
    {
        HttpListenerContext context;
        try { context = await listener.GetContextAsync(); } catch { return; }
        using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();
        var path = context.Request.Url!.AbsolutePath;
        seen.Enqueue($"{context.Request.HttpMethod} {path}{context.Request.Url.Query} {context.Request.Headers["x-api-key"]} {body}");
        var reply = path.StartsWith("/v1/messages/") ? "{\"id\":\"3f2504e0-4f89-41d3-9a0c-0305e82c3301\",\"status\":\"queued\"}" : "42";
        context.Response.StatusCode = path.StartsWith("/v1/messages/") ? 202 : 200;
        context.Response.ContentType = path.StartsWith("/v1/messages/") ? "application/json" : "text/plain";
        var bytes = Encoding.UTF8.GetBytes(reply);
        await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
        context.Response.Close();
    }
});

var sms = new SmsClient(new SmsClientOptions { Username = "consumer-user", Password = "consumer-pass", BaseUri = baseUri });
var calls = new SwitchClient(new SwitchClientOptions { Key = "consumer-key", BaseUri = baseUri });
var whatsApp = new WhatsAppClient(new WhatsAppClientOptions { ApiKey = "consumer-api-key", BaseUri = baseUri });

Require(await sms.BalanceAsync() == "42", "SMS balance");
Require(await calls.OriginateAsync("1001", "905551112233") == "42", "Switch originate");
Require((await whatsApp.SendOtpAsync("905551112233", "otp")).Status == "queued", "WhatsApp OTP");

var log = string.Join("\n", seen);
Require(log.Contains("GET /v2/balance?password=consumer-pass&username=consumer-user"), "SMS credentials in query");
Require(log.Contains("POST /originate?key=consumer-key"), "Switch key in query");
Require(log.Contains("POST /v1/messages/otp consumer-api-key"), "WhatsApp key in header");
Console.WriteLine("Installed package consumer passed for SMS, Switch and WhatsApp.");
return 0;

static void Require(bool condition, string what)
{
    if (!condition)
    {
        Console.Error.WriteLine("Consumer check failed: " + what);
        Environment.Exit(1);
    }
}
