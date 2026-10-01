# SMS

```csharp
var sms = new SmsClient(new SmsClientOptions { Username = "...", Password = "...", DefaultSender = "VERIMOR" });

await sms.SendAsync("905000000000", "Merhaba");                       // varsayılan başlık
await sms.SendAsync("905000000000", "Merhaba", sourceAddr: "DIGER");  // çağrı başına başlık
await sms.BalanceAsync();
await sms.StatusByIdAsync(12345);
await sms.StatusByCustomIdAsync("siparis-42");
```

Ayrıntılı gönderim için üretilmiş istek modelini kullanın. Modelin `username` ve `password` alanlarına boş dize verin; istemci kendi kimlik bilgilerini doldurur:

```csharp
using BarisCemant.Verimor.Sms.Generated.Model;

var request = new SendSmsJsonRequest(
    string.Empty,
    string.Empty,
    new List<SendSmsJsonRequestMessagesInner> { new SendSmsJsonRequestMessagesInner("905000000000", "Merhaba") });
await sms.Campaigns.SendAsync(request);
```

Servisler: `Balances`, `Blacklist`, `Campaigns`, `Iys`, `Reports`, `SenderIds`. Tüm metotlar [operasyon tablosunda](operations.md).
