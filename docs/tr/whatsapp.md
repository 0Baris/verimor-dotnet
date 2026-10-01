# WhatsApp

```csharp
var whatsApp = new WhatsAppClient(new WhatsAppClientOptions { ApiKey = "..." });

var otp = await whatsApp.SendOtpAsync("905000000000", "otp_template", "tr", new[] { "123456" });
var utility = await whatsApp.SendUtilityAsync("905000000000", "order_update", "tr", new[] { "42" });
Console.WriteLine($"{otp.Id} {otp.Status}");
```

Mesaj uçları `202 Accepted` döner. Gövde beklenen `MessageResponse` şeklinde değilse `UnexpectedResponseException` atılır. `Health.HealthAsync()` kimlik bilgisi göndermez.
