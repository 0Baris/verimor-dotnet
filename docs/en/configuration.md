# Configuration

Each product has its own options class; all derive from `ClientOptions`.

| Option | Meaning |
| --- | --- |
| `BaseUri` | Server address. Defaults to Verimor's address for the product; set it to use another server such as a proxy. An IP, a port and a path prefix are kept. |
| `Timeout` | Per-request time limit; 30 seconds by default. Zero or negative values are rejected. |
| `HttpClient` | A client you own. The SDK never disposes it and never touches its `Timeout` or default headers. |

Product-specific fields:

- `SmsClientOptions`: `Username`, `Password`, `DefaultSender` (`source_addr`).
- `SwitchClientOptions`: `Key`.
- `WhatsAppClientOptions`: `ApiKey` (the `x-api-key` header).

Blank credentials are rejected with `ArgumentException` when the client is created; the message never contains the secret. Each client sends only its own credentials.

The SDK never retries. If you need retries, add them in your application and only for operations that are safe to repeat (a repeated SMS send may deliver twice).
