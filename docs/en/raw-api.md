# Raw access

Every client's `Raw` property reaches all of the product's operations by operation id. Credentials and the default SMS sender are still added by the client.

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

An unknown operation id is rejected with `ArgumentException` before any request is sent.
