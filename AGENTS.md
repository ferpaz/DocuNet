# Repository Guidelines

## Purpose and architecture

DocuNet is a .NET 10 CLI that generates static HTML documentation from .NET Framework or modern .NET assemblies and their XML comments. Currently validated support covers Windows and Linux.

Separate metadata reading, XML parsing, the documentation model and presentation. Inspect assemblies without executing their code or attribute constructors. Preserve complete XML identifiers to resolve members, references and overloads.

The current contract accepts assemblies/XML, --output and --templates. Do not add mandatory product-specific parameters, Git integration, a backend or deployment without first defining the scope change.

## Structure

Keep the solution, projects and development configuration inside src:

- DocuNet.Cli: arguments and diagnostics.
- DocuNet.Core: DLL/XML reading and model.
- DocuNet.Rendering: templates and static file generation.
- tests/DocuNet.Tests: xUnit tests.
- tests/DocuNet.TestFixtures: buildable synthetic assemblies.

Public documentation belongs in docs; additional test data in tests/fixtures and regenerable results in tests/results. Local historical analysis/requirement records and private samples are ignored; they are not required to contribute.

See [source layout](docs/source-layout.md). Do not commit private DLLs, credentials, sensitive documentation or build output.

## Development

Run from src to apply global.json:

```shell
dotnet restore --locked-mode
dotnet build
dotnet test
dotnet run --project DocuNet.Cli -- --help
```

Use C#, four spaces, nullable reference types, PascalCase for public types/members and camelCase for parameters/variables. Follow .editorconfig and use cross-platform path APIs.

## Tests and files

Use xUnit.net and NSubstitute for simulated collaborators; use real fixtures and files to verify metadata and rendering. Maintain at least 90% line coverage and 80% branch coverage per production project; see [testing](docs/testing.md).

Cover generics, overloads, missing/invalid XML, missing dependencies, HTML escaping and link targets. Verify deterministic regeneration and preservation of unrelated or manually modified files. Do not write outside the requested destination or modify original samples.

## Presentation and licenses

Templates are trusted C# code and must remain externally editable. New templates must preserve IDs/content and be tested for navigation, responsive behavior and accessibility. jQuery and the legacy design will be modernized according to the [roadmap](docs/roadmap.md).

Preserve Docu attribution and third-party notices. If dependencies or copied resources change, update [notices](THIRD-PARTY-NOTICES.md) and the licenses distributed with the CLI and site. DocuNet's MIT license does not replace third-party licenses.

## Collaboration

Follow [CONTRIBUTING.md](CONTRIBUTING.md) and [SECURITY.md](SECURITY.md). Use GitHub Issues for bugs/proposals and link PRs when appropriate. Commit messages must describe the change; do not require local requirement record codes.

Write public documentation and new explanatory text in English. Preserve original third-party license and attribution text.

For significant changes, agree on scope and criteria before implementing. Describe changes and validation; attach screenshots for presentation changes. Do not push, publish or deploy without authorization. The local AI protocol in docs/harness applies only when explicitly requested; it is not required for public contributions.