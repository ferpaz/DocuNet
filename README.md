# DocuNet

Generate static HTML documentation for .NET libraries from their assemblies and XML comments.

DocuNet is a command-line tool written in C# and .NET 10. It reads metadata without executing the documented assembly's code. The resulting site can be browsed locally or published to static hosting, with no backend.

[View the live demo](https://ferpaz.github.io/DocuNet/) — generated from the public synthetic fixture. See [publishing and local demo instructions](docs/demo.md).

## Features

- Documents public types and members: properties, fields, methods and events, including inherited members.
- Preserves XML identifiers for generics, overloads and references between members.
- Accepts .NET Framework and modern .NET DLL/EXE inputs.
- Generates relative links and unique anchors for overloads.
- Supports HTML, CSS and JavaScript customization through external Spark templates, without recompiling.
- Regenerates managed files while protecting unrelated files and manual changes.

## Requirements

- A .NET 10 SDK compatible with [src/global.json](src/global.json).
- A managed DLL or EXE and its XML documentation file.
- Dependencies required to resolve base classes, alongside the input assembly.

The tool has been tested on Windows and Linux. Planned platform support is described in the [roadmap](docs/roadmap.md#distribution-by-platform-and-architecture).

To produce XML documentation from a C# project, enable this property:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

PDB files are not required.

## Build and test

From a local copy of the repository:

```shell
cd src
dotnet restore --locked-mode
dotnet build
dotnet test
dotnet run --project DocuNet.Cli -- --help
```

The solution and all projects are under src. Tests use xUnit.net and NSubstitute and do not require private assemblies.

## Quick start

After building, generate a sample site using the included fixture. Run from src:

```shell
dotnet run --no-build --project DocuNet.Cli -- --output=../output/demo tests/DocuNet.TestFixtures/bin/Debug/net10.0/DocuNet.TestFixtures.dll
```

Open output/demo/index.htm from the repository root.

To document your library:

```shell
dotnet run --project DocuNet.Cli -- --output=../output/library "/path/MyLibrary.dll"
```

If you do not specify XML files, DocuNet looks for a companion file with the same name as each assembly. You can also provide them explicitly:

```shell
dotnet run --project DocuNet.Cli -- --output=../output/library "/path/MyLibrary.dll" "/path/MyLibrary.xml"
```

Relative paths are resolved from the current directory. Replace the example paths with paths on your machine.

## CLI options

| Option or input | Behavior |
| --- | --- |
| --help | Displays help |
| --output=path | Output directory; default: ./output |
| --templates=path | Template directory; default: Templates alongside the executable |
| assembly.dll / assembly.exe | One or more managed assemblies |
| documentation.xml | One or more XML documentation files |
| * and ? patterns | Select input files; quote patterns so the CLI processes them |

Exit codes are 0 for success/help, 2 for arguments or inputs, 3 for metadata/resolution/XML, and 4 for templates or writing output.

## Customize the presentation

Copy src/DocuNet.Rendering/Templates to your own directory and edit its .spark, CSS and JavaScript files. From src:

```shell
dotnet run --project DocuNet.Cli -- --templates=../my-templates --output=../output/custom "/path/MyLibrary.dll"
```

The template directory must contain a complete copy of the required templates and resources. You can regenerate the site after editing them without recompiling DocuNet. Templates compile C#: use trusted templates.

The data contract, helpers and name expansion rules are documented in [architecture and templates](docs/architecture.md).

## Regeneration and publishing

Output includes .docunet-manifest.json, which identifies managed files. Keep this manifest: DocuNet rejects collisions with unrelated files and manual changes to generated files. Customize templates to regenerate the site.

Rendering is prepared before output is replaced. The final copy of multiple files is not an atomic transaction against system interruptions. Linked paths or reparse points are not supported for templates or output.

Publish the generated content using your preferred static hosting provider. The CLI generates files; it does not commit, push or deploy.

## Documentation

- [Demo and GitHub Pages publishing](docs/demo.md).
- [Architecture and template contract](docs/architecture.md).
- [Source layout](docs/source-layout.md).
- [Technology](docs/technology.md).
- [Testing and coverage](docs/testing.md).
- [Roadmap](docs/roadmap.md).
- [Contributing](CONTRIBUTING.md).
- [Security policy](SECURITY.md).
- [Third-party notices and licenses](THIRD-PARTY-NOTICES.md).

## Current limitations

Inheritance resolution uses .NET Framework 4.8 references for Framework inputs and the CLI's .NET platform for modern inputs; the inherited API surface may differ from other runtimes. inheritdoc is not expanded, and function-pointer signatures are not supported. See the architecture documentation for details.

## Acknowledgments

[Docu](https://github.com/jagregory/docu), created by James Gregory, is the inspiration for DocuNet and the basis of its legacy templates, adapted for this project. Its attribution and license are preserved alongside the notices for third-party resources. DocuNet is an independent project; this acknowledgment does not imply endorsement by the original authors.

## License

DocuNet is distributed under the [MIT license](license.md).

Copyright (c) 2026 Fernando Paz. You may use, modify and redistribute the code, including commercially, while preserving the copyright notice and license text. Third-party components retain their own notices and terms.
