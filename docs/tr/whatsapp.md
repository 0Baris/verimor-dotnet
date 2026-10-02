# WhatsApp

```csharp
var whatsApp = new WhatsAppClient(new WhatsAppClientOptions { ApiKey = "..." });

var otp = await whatsApp.SendOtpAsync("905000000000", "otp_template", "tr", new[] { "123456" });
var utility = await whatsApp.SendUtilityAsync("905000000000", "order_update", "tr", new[] { "42" });
Console.WriteLine($"{otp.Id} {otp.Status}");
```

Mesaj uçları `202 Accepted` döner. Gövde beklenen `MessageResponse` şeklinde değilse `UnexpectedResponseException` atılır. `Health.HealthAsync()` kimlik bilgisi göndermez.

`Messages` ayrıca tek şablonu en fazla 10.000 alıcıya kuyruğa alan `SendBulkAsync(BulkMessageRequest)`, gönderilen mesajları filtreleyen `ListMessagesAsync(...)` ve tek mesajın güncel durumunu döndüren `GetMessageAsync(messageRef)` metotlarını sunar.
