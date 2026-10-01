# WhatsApp

```csharp
var whatsApp = new WhatsAppClient(new WhatsAppClientOptions { ApiKey = "..." });

var otp = await whatsApp.SendOtpAsync("905000000000", "otp_template", "en", new[] { "123456" });
var utility = await whatsApp.SendUtilityAsync("905000000000", "order_update", "en", new[] { "42" });
Console.WriteLine($"{otp.Id} {otp.Status}");
```

Message endpoints return `202 Accepted`. If the body is not the expected `MessageResponse`, `UnexpectedResponseException` is raised. `Health.HealthAsync()` sends no credentials.
