# Source layout

The structure keeps projects and development configuration together.

All source code and files governing its development belong inside src/:

```text
src/
    DocuNet.slnx
    Directory.Build.props
    Directory.Packages.props
    global.json
    .editorconfig
    .gitignore
    .gitattributes
    DocuNet.Cli/
    DocuNet.Core/
    DocuNet.Rendering/
    tests/
        Directory.Packages.props
        DocuNet.Tests/
        DocuNet.TestFixtures/
tests/
    fixtures/
    results/
docs/
    requirements/
```

- The solution and all projects, including test projects, remain inside src/.
- src/Directory.Build.props contains shared project properties.
- src/Directory.Packages.props enables central NuGet management and contains versions for Cli, Core and Rendering. Projects under src/tests/ automatically use their independent src/tests/Directory.Packages.props catalog, without importing the production catalog.
- src/global.json selects the SDK; run development commands from src/ so it applies.
- src/.editorconfig defines code conventions.
- src/.gitattributes preserves legacy resources and templates without line-ending conversion or normalization of their original formatting.
- src/.gitignore contains exclusions for .NET/C#, JetBrains Rider, Visual Studio Code, Visual Studio and operating systems. It was generated from [Toptal/gitignore.io](https://www.toptal.com/developers/gitignore/api/rider,visualstudio,visualstudiocode,csharp,dotnetcore,windows,linux,macos). The service provides the generic VisualStudio template for Visual Studio 2026.
- tests/fixtures/ and tests/results/ at the repository root contain harness inputs and evidence, not test projects.
- docs/ contains documentation and docs/requirements/ contains requirement records. Private samples remain outside the source tree and are ignored under samples/.

From the repository root:

```powershell
cd src
dotnet restore
dotnet build
dotnet test
dotnet run --project DocuNet.Cli -- --help
```

Keep this structure when adding projects or changing development instructions.
