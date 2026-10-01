# Test ve güvenlik

- Testler gerçek Verimor kimlik bilgisi kullanmaz ve canlı servise bağlanmaz; tüm HTTP testleri `127.0.0.1` üzerindeki yerel sunucuya gider.
- Kendi uygulamanızı test ederken `BaseUri` ile yerel bir sunucuya yönlendirin; gerçek SMS veya arama maliyet doğurabilir.
- Kimlik bilgilerini kaynak koduna yazmayın; ortam değişkeni veya secret manager kullanın.
- Bu sürüm canlı Verimor servisine karşı doğrulanmadı. Üretime almadan önce kendi test hesabınızla küçük bir deneme yapın.

Depoyu doğrulamak için:

```bash
dotnet test tests/BarisCemant.Verimor.Tests
```
