# Değişiklik günlüğü / Changelog

Bu proje [Semantic Versioning](https://semver.org/) kullanır. / This project follows Semantic Versioning.

## 0.3.0

- 72 operasyonun her biri için çalıştırılabilir örnek: `examples/Operations/<Ürün>/<Operasyon>.cs`; biri `dotnet run --project examples -- sms/send` ile çalışır. Örnekler sözleşmeden üretilir; CI hepsini Verimor'a bağlanmadan yerel bir kayıt sunucusuna karşı çalıştırıp her birinin belgelenen isteği gönderdiğini doğrular (`scripts/run_examples.py dotnet`).
- Yapay zekâ asistanları için tek dosyalık başvuru: `llms.md`. Kütüphane kodu ve API'ler 0.2.1 ile aynıdır.
- Runnable example for each of the 72 operations: `examples/Operations/<Product>/<Operation>.cs`; run one with `dotnet run --project examples -- sms/send`. The examples are generated from the contract; CI runs every one against a local recording server, without contacting Verimor, and checks each sends the documented request (`scripts/run_examples.py dotnet`).
- A single-file reference for AI assistants: `llms.md`. Library code and APIs are unchanged from 0.2.1.

## 0.2.1

- Sunucu adresi açıklamaları netleşti: varsayılan Verimor'un adresidir; kendi sunucunuz veya proxy için değiştirilebilir, IP, port ve alt yol korunur (testle doğrulandı).
- Server URL docs clarified: Verimor's address is the default and can be changed to your own server or proxy; an IP, a port and a path prefix are kept (now tested).

## 0.2.0

- Verimor'un yeni operasyonları: SMS `Campaigns.SendOtpAsync` (`POST /v2/otp`); WhatsApp `Messages.SendBulkAsync`, `ListMessagesAsync` ve `GetMessageAsync`. Kapsam 72 operasyon: SMS 14, Switch 52, WhatsApp 6.
- Varsayılan değeri olan isteğe bağlı enum alanları ayarlanmadığında artık gönderilmez; önceden OTP `lang` boş bırakılınca serileştirme hata veriyordu.
- Verimor's new operations: SMS `Campaigns.SendOtpAsync` (`POST /v2/otp`); WhatsApp `Messages.SendBulkAsync`, `ListMessagesAsync` and `GetMessageAsync`. Coverage is 72 operations: 14 SMS, 52 Switch, 6 WhatsApp.
- Optional enum fields with a default are no longer written when unset; leaving OTP `lang` empty used to fail serialization.

## 0.1.0 - Yayın adayı / Release candidate

- `netstandard2.0` hedefli SMS, Switch ve WhatsApp istemcileri.
- Alan servisleri ve `Raw` erişimiyle 68 operasyonun tamamı.
- HTTP hata normalizasyonu, 30 saniyelik zaman aşımı ve otomatik tekrar içermeyen güvenli varsayılanlar.
- Offline sözleşme, localhost, paket içeriği ve temiz consumer testleri.

Canlı Verimor servisi doğrulaması ve NuGet yayını henüz yapılmamıştır.

- SMS, Switch and WhatsApp clients targeting `netstandard2.0`.
- All 68 operations through domain services and `Raw` access.
- HTTP error normalization, a 30-second timeout and no automatic retries.
- Offline contract, localhost, package-content and clean-consumer tests.

Live Verimor validation and NuGet publication have not been performed yet.
