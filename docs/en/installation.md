# Installation

```bash
dotnet add package BarisCemant.Verimor
```

The package targets `netstandard2.0` only; .NET Framework 4.6.2+, .NET 8 and .NET 10 projects use the same assembly. Dependencies: `System.Text.Json` and `System.ComponentModel.Annotations`. Polly, `Microsoft.Extensions.Hosting` and third-party HTTP clients are not pulled in.

On language versions before C# 8 the nullable annotations are ignored; the API stays the same.
