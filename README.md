# Verimor .NET SDK

[English](README.en.md)

Verimor SMS, Switch ve WhatsApp API'leri için `netstandard2.0` hedefli, bağımsız topluluk SDK'sı. .NET Framework 4.6.2+, .NET 8 ve .NET 10 uygulamalarında çalışır. 72 operasyonun tamamı alan servisleri ve `Raw` erişimiyle kullanılabilir.

> Bu proje topluluk tarafından sürdürülür ve resmî değildir. Verimor adına destek veya uyumluluk garantisi vermez.
>
> Bu sürüm offline sözleşme ve localhost testleriyle doğrulanmıştır; canlı Verimor servisine karşı henüz doğrulanmamıştır.

## Kurulum

```bash
dotnet add package BarisCemant.Verimor
```

Kimlik bilgilerini yalnız sunucu tarafında, ortam değişkeni veya secret manager içinde tutun.

## Hızlı başlangıç

### SMS

```csharp
using BarisCemant.Verimor.Sms;

var sms = new SmsClient(new SmsClientOptions
{
    Username = Environment.GetEnvironmentVariable("VERIMOR_SMS_USERNAME")!,
    Password = Environment.GetEnvironmentVariable("VERIMOR_SMS_PASSWORD")!,
    DefaultSender = "VERIMOR",
});

var campaignId = await sms.SendAsync("905000000000", "Merhaba");
var balance = await sms.BalanceAsync();
var statuses = await sms.StatusByIdAsync(12345);
// veya: await sms.StatusByCustomIdAsync("siparis-42");
```

`DefaultSender` her gönderimde `source_addr` olarak kullanılır; tek çağrıda `sourceAddr:` vererek değiştirebilirsiniz. İkisi de yoksa alan gönderilmez.

### Switch

```csharp
using BarisCemant.Verimor.Switch;

var calls = new SwitchClient(new SwitchClientOptions
{
    Key = Environment.GetEnvironmentVariable("VERIMOR_SWITCH_API_KEY")!,
});
var callId = await calls.OriginateAsync(extension: "101", destination: "905000000000");
```

Switch operasyonları alan servislerindedir: `Calls`, `Contacts`, `Queues`, `Records`, `Users` ve diğerleri.

### WhatsApp

```csharp
using BarisCemant.Verimor.WhatsApp;

var whatsApp = new WhatsAppClient(new WhatsAppClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("VERIMOR_WHATSAPP_API_KEY")!,
});
var accepted = await whatsApp.SendOtpAsync("905000000000", "otp_template", "tr", new[] { "123456" });
```

## Davranış

- Varsayılan zaman aşımı 30 saniyedir; SDK hiçbir isteği otomatik tekrarlamaz.
- Kendi `HttpClient`'ınızı `HttpClient` seçeneğiyle verebilirsiniz; SDK onu kapatmaz ve ayarlarını değiştirmez.
- 2xx dışındaki yanıtlar `VerimorApiException`, beklenmeyen 2xx gövdeleri `UnexpectedResponseException` olur. Ağ ve iptal hataları .NET'in kendi istisnaları olarak kalır.

## Örnekler

72 operasyonun her biri için çalıştırılabilir bir örnek [`examples/Operations/`](examples/Operations/) altındadır; biri şöyle çalışır: `dotnet run --project examples -- sms/send`. Her dosya kimlik bilgilerini ortam değişkenlerinden, sunucu adresini `VERIMOR_BASE_URL` değişkeninden okur; değişken yoksa Verimor'un sunucusuna gider. `scripts/run_examples.py` hepsini Verimor'a hiç bağlanmadan yerel bir kayıt sunucusuna karşı çalıştırır.

Yapay zekâ asistanları için tek dosyalık başvuru: [`llms.md`](llms.md).

## Belgeler

- [Kurulum](docs/tr/installation.md) · [Yapılandırma](docs/tr/configuration.md) · [SMS](docs/tr/sms.md) · [Switch](docs/tr/switch.md) · [WhatsApp](docs/tr/whatsapp.md)
- [Hatalar](docs/tr/errors.md) · [Raw erişim](docs/tr/raw-api.md) · [Test ve güvenlik](docs/tr/testing-and-safety.md) · [Tüm operasyonlar](docs/tr/operations.md)

## Lisans

MIT. Bkz. [LICENSE](LICENSE).
