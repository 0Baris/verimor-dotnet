# Hatalar

| Durum | İstisna |
| --- | --- |
| HTTP 2xx dışı | `VerimorApiException` (`StatusCode`, `BodyKind`, `Body`) |
| 2xx ama gövde boş, JSON değil veya beklenen şekilde değil | `UnexpectedResponseException` |
| Ağ hatası | `HttpRequestException` (sarılmaz) |
| İptal veya zaman aşımı | `OperationCanceledException` / `TaskCanceledException` (sarılmaz) |
| Boş kimlik bilgisi, eksik yol değeri | `ArgumentException` |

`BodyKind` değerleri: `Json`, `Text`, `Empty`, `Binary`. JSON hata gövdesindeki `message`, `detail`, `error` veya `msg` alanı mesaja eklenir. Mesajlar kimlik bilgisi veya istek adresi içermez. Hiçbir hata durumunda istek tekrarlanmaz.

```csharp
try
{
    await sms.BalanceAsync();
}
catch (VerimorApiException error) when (error.StatusCode == 401)
{
    // kimlik bilgilerini kontrol edin
}
```
