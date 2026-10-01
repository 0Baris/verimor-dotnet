# SMS

```csharp
var sms = new SmsClient(new SmsClientOptions { Username = "...", Password = "...", DefaultSender = "VERIMOR" });

await sms.SendAsync("905000000000", "Hello");                        // default sender
await sms.SendAsync("905000000000", "Hello", sourceAddr: "OTHER");   // per-call sender
await sms.BalanceAsync();
await sms.StatusByIdAsync(12345);
await sms.StatusByCustomIdAsync("order-42");
```

For detailed sends use the generated request model. Pass empty strings for its `username` and `password`; the client fills in its own credentials:

```csharp
using BarisCemant.Verimor.Sms.Generated.Model;

var request = new SendSmsJsonRequest(
    string.Empty,
    string.Empty,
    new List<SendSmsJsonRequestMessagesInner> { new SendSmsJsonRequestMessagesInner("905000000000", "Hello") });
await sms.Campaigns.SendAsync(request);
```

Services: `Balances`, `Blacklist`, `Campaigns`, `Iys`, `Reports`, `SenderIds`. Every method is in the [operation table](operations.md).
