# Switch

```csharp
var calls = new SwitchClient(new SwitchClientOptions { Key = "..." });

await calls.OriginateAsync(extension: "101", destination: "905000000000");
await calls.Calls.HangupAsync("call-id");
var queues = await calls.Queues.ListQueuesAsync();
var cdrs = await calls.Records.ListCdrsAsync(page: 1, limit: 50);
```

Services: `Announcements`, `Blacklist`, `CallerIds`, `Calls`, `Contacts`, `Crm`, `Fax`, `IvrCampaigns`, `Queues`, `Records`, `Users`. The fax document download returns `byte[]`. Every method is in the [operation table](operations.md).
