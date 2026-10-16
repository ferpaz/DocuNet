# Contributing to DocuNet

Contributions can improve the metadata reader, documentation, tests or site templates.

## Tracking work

Use GitHub Issues to report bugs, propose improvements and agree on the scope of changes. Link pull requests to their issue when appropriate. Local historical records and results are not required to contribute and must not be attached to the public repository.

For vulnerabilities, follow [SECURITY.md](SECURITY.md) and use a private channel.

## Reporting a problem

Describe the expected and observed behavior, the command used, operating system, SDK version and diagnostics. If possible, include a minimal buildable project that reproduces the problem. Do not publish private DLLs, credentials or sensitive documentation.

## Preparing a change

1. Review README.md and the architecture documentation.
2. Build and run tests from src.
3. Keep assembly processing separate from rendering.
4. Add a behavioral regression test when fixing a defect.
5. Update documentation if the CLI or template contract changes.

For interface changes, attach screenshots and describe navigation, viewport and accessibility checks. For large changes, agree on scope with the maintainers first.

## Checks

```shell
cd src
dotnet restore --locked-mode
dotnet build
dotnet test
```

See [testing and coverage](docs/testing.md) to measure coverage and check its thresholds. Do not include bin/obj, generated sites, local reports or private samples.

## Conventions

Use C#, four spaces, nullable reference types, PascalCase for public types and members and camelCase for parameters and variables. Keep projects and development configuration inside src.

Write public documentation and new explanatory text in English. Preserve original third-party notices and license text.

Explain what changes, why and how it was verified in your contribution. Preserve authorship and third-party component notices.
