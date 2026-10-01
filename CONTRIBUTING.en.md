# Contributing

[Türkçe](CONTRIBUTING.md)

`src/BarisCemant.Verimor/*/Generated/`, `*/Services/*.gen.cs`, `*/RawOperations.gen.cs`, `*/*Client.gen.cs`, `contracts/operations.json` and `docs/*/operations.md` are generated; do not edit them by hand. Defects in them are fixed at the source and re-exported. Every other file belongs to this repository.

Before sending a change:

```bash
dotnet build -c Release
dotnet test tests/BarisCemant.Verimor.Tests
```

Tests must not use real credentials or the live service.
