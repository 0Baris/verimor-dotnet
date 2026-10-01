# Errors

| Situation | Exception |
| --- | --- |
| Non-2xx HTTP status | `VerimorApiException` (`StatusCode`, `BodyKind`, `Body`) |
| 2xx with an empty, non-JSON or wrongly shaped body | `UnexpectedResponseException` |
| Network failure | `HttpRequestException` (not wrapped) |
| Cancellation or timeout | `OperationCanceledException` / `TaskCanceledException` (not wrapped) |
| Blank credentials, missing path value | `ArgumentException` |

`BodyKind` is one of `Json`, `Text`, `Empty`, `Binary`. A `message`, `detail`, `error` or `msg` field in a JSON error body is added to the message. Messages never contain credentials or the request address. No error is ever retried.

```csharp
try
{
    await sms.BalanceAsync();
}
catch (VerimorApiException error) when (error.StatusCode == 401)
{
    // check the credentials
}
```
