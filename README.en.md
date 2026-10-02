# Verimor .NET SDK

[Türkçe](README.md)

An independent community SDK for the Verimor SMS, Switch and WhatsApp APIs, targeting `netstandard2.0`. It runs on .NET Framework 4.6.2+, .NET 8 and .NET 10. All 72 operations are available through domain services and `Raw` access.

> This project is community-maintained and unofficial. It provides no support or compatibility guarantee on behalf of Verimor.
>
> This release is verified with offline contract and localhost tests; it has not been validated against the live Verimor services yet.

## Installation

```bash
dotnet add package BarisCemant.Verimor
```

Keep credentials on the server side only, in environment variables or a secret manager.

## Quick start

### SMS

```csharp
using BarisCemant.Verimor.Sms;

var sms = new SmsClient(new SmsClientOptions
{
    Username = Environment.GetEnvironmentVariable("VERIMOR_SMS_USERNAME")!,
    Password = Environment.GetEnvironmentVariable("VERIMOR_SMS_PASSWORD")!,
    DefaultSender = "VERIMOR",
});

var campaignId = await sms.SendAsync("905000000000", "Hello");
var balance = await sms.BalanceAsync();
var statuses = await sms.StatusByIdAsync(12345);
// or: await sms.StatusByCustomIdAsync("order-42");
```

`DefaultSender` is sent as `source_addr` on every send; pass `sourceAddr:` to override it for one call. When neither is set the field is omitted.

### Switch

```csharp
using BarisCemant.Verimor.Switch;

var calls = new SwitchClient(new SwitchClientOptions
{
    Key = Environment.GetEnvironmentVariable("VERIMOR_SWITCH_API_KEY")!,
});
var callId = await calls.OriginateAsync(extension: "101", destination: "905000000000");
```

Switch operations live in domain services: `Calls`, `Contacts`, `Queues`, `Records`, `Users` and more.

### WhatsApp

```csharp
using BarisCemant.Verimor.WhatsApp;

var whatsApp = new WhatsAppClient(new WhatsAppClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("VERIMOR_WHATSAPP_API_KEY")!,
});
var accepted = await whatsApp.SendOtpAsync("905000000000", "otp_template", "en", new[] { "123456" });
```

## Behavior

- The default timeout is 30 seconds; the SDK never retries a request.
- Pass your own `HttpClient` through the `HttpClient` option; the SDK never disposes it or changes its settings.
- Non-2xx responses raise `VerimorApiException`, unexpected 2xx bodies raise `UnexpectedResponseException`. Network and cancellation errors stay the native .NET exceptions.

## Examples

Every one of the 72 operations has a runnable example under [`examples/Operations/`](examples/Operations/); run one with `dotnet run --project examples -- sms/send`. Each file reads credentials from environment variables and the server from `VERIMOR_BASE_URL`, falling back to Verimor's server. `scripts/run_examples.py` runs all of them against a local recording server and never contacts Verimor.

A single-file reference for AI assistants: [`llms.md`](llms.md).

## Documentation

- [Installation](docs/en/installation.md) · [Configuration](docs/en/configuration.md) · [SMS](docs/en/sms.md) · [Switch](docs/en/switch.md) · [WhatsApp](docs/en/whatsapp.md)
- [Errors](docs/en/errors.md) · [Raw access](docs/en/raw-api.md) · [Testing and safety](docs/en/testing-and-safety.md) · [All operations](docs/en/operations.md)

## License

MIT. See [LICENSE](LICENSE).
