# Raw erişim

Her istemcinin `Raw` özelliği, ürünün tüm operasyonlarına operasyon kimliğiyle erişim verir. Kimlik bilgileri ve varsayılan SMS başlığı yine istemci tarafından eklenir.

```csharp
var request = new RawRequest();
request.Query["page"] = "1";
RawResponse response = await calls.Raw.SendAsync("getCdrs", request);
Console.WriteLine(response.Body);

foreach (var operation in calls.Raw.Operations)
{
    Console.WriteLine($"{operation.OperationId} {operation.Method} {operation.PathTemplate}");
}
```

Bilinmeyen operasyon kimliği istek gönderilmeden `ArgumentException` ile reddedilir.
