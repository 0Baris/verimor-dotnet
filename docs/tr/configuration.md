# Yapılandırma

Her ürünün kendi seçenek sınıfı vardır ve hepsi `ClientOptions`'tan türer.

| Seçenek | Açıklama |
| --- | --- |
| `BaseUri` | Sunucu adresi. Varsayılan olarak ürünün Verimor adresidir; proxy gibi başka bir sunucu kullanmak için değiştirin. IP, port ve alt yol korunur. |
| `Timeout` | İstek başına süre sınırı; varsayılan 30 saniye. Sıfır veya negatif değer reddedilir. |
| `HttpClient` | Sizin yönettiğiniz istemci. SDK onu kapatmaz; `Timeout` ve varsayılan başlıklarına dokunmaz. |

Ürüne özel alanlar:

- `SmsClientOptions`: `Username`, `Password`, `DefaultSender` (`source_addr`).
- `SwitchClientOptions`: `Key`.
- `WhatsAppClientOptions`: `ApiKey` (`x-api-key` başlığı).

Boş kimlik bilgisi istemci oluşturulurken `ArgumentException` ile reddedilir; hata mesajı gizli değeri içermez. Her istemci yalnız kendi kimlik bilgilerini gönderir.

SDK otomatik tekrar denemez. Tekrar gerekiyorsa uygulamanızda, işlemin tekrarlanabilir olduğundan emin olarak yapın (ör. SMS gönderimi tekrarlanırsa mesaj iki kez gidebilir).
