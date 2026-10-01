# Kurulum

```bash
dotnet add package BarisCemant.Verimor
```

Paket yalnız `netstandard2.0` hedefler; .NET Framework 4.6.2+, .NET 8 ve .NET 10 projeleri aynı derlemeyi kullanır. Bağımlılıklar: `System.Text.Json` ve `System.ComponentModel.Annotations`. Polly, `Microsoft.Extensions.Hosting` veya üçüncü taraf HTTP istemcisi gelmez.

C# 8 öncesi dil sürümlerinde nullable açıklamaları yok sayılır; API aynı kalır.
