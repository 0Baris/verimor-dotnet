# Katkı

[English](CONTRIBUTING.en.md)

`src/BarisCemant.Verimor/*/Generated/`, `*/Services/*.gen.cs`, `*/RawOperations.gen.cs`, `*/*Client.gen.cs`, `contracts/operations.json` ve `docs/*/operations.md` üretilmiş dosyalardır; elle değiştirmeyin. Bu dosyalardaki hatalar kaynak tarafta düzeltilip yeniden dışa aktarılır. Diğer tüm dosyalar bu depoya aittir.

Değişiklik göndermeden önce:

```bash
dotnet build -c Release
dotnet test tests/BarisCemant.Verimor.Tests
```

Testler gerçek kimlik bilgisi veya canlı servis kullanmamalıdır.
