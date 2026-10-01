# Testing and safety

- Tests never use real Verimor credentials and never reach the live service; all HTTP tests go to a local server on `127.0.0.1`.
- When testing your own application, point `BaseUri` at a local server; real SMS or calls may cost money.
- Do not put credentials in source code; use environment variables or a secret manager.
- This release has not been validated against the live Verimor services. Try a small run with your own test account before production use.

To verify the repository:

```bash
dotnet test tests/BarisCemant.Verimor.Tests
```
