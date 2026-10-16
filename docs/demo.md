# Public demo on GitHub Pages

The demo is generated from src/tests/DocuNet.TestFixtures, which contains synthetic code and XML comments included in the repository. It demonstrates generics, inheritance, overloads, members and references; it does not represent documentation for a private product.

## Automation

The [Publish demo](../.github/workflows/demo.yml) workflow runs on changes to master or manually through workflow_dispatch, always on master.

1. Restores locked dependencies, builds and runs tests with coverage.
2. Generates the site using the CLI and public fixture.
3. Copies index.htm to index.html for the default GitHub Pages entry point.
4. Uploads only the generated directory as the Pages artifact.
5. Publishes through the github-pages environment.

Generated HTML is not added to master and does not require a gh-pages branch. The artifact contains the site and its third-party notices; it does not contain private samples, fixture assemblies, repository files or coverage reports.

Relative links work under the repository subdirectory. The actual URL comes from deployment output and appears in the github-pages environment and workflow page; it does not depend on a hard-coded username or repository name.

## Enable publishing

Once the public repository has been created and changes pushed:

1. Open Settings → Pages.
2. Under Build and deployment, select Source: GitHub Actions.
3. Review github-pages environment rules to allow deployments from master.
4. Run Publish demo from Actions on master, or push a change to that branch.
5. Check the URL emitted by deployment and update the README's "View the demo" link to that address.

The remote demo is not currently published. The prepared workflow and local checks do not establish that GitHub Actions has run. The README links to these instructions until the actual URL is available.

Official documentation: [GitHub Pages workflows](https://docs.github.com/en/pages/getting-started-with-github-pages/using-custom-workflows-with-github-pages).

## Generate the demo locally

From src:

```powershell
dotnet restore --locked-mode
dotnet build --configuration Release
dotnet run --no-build --configuration Release --project DocuNet.Cli -- --output=../output/demo tests/DocuNet.TestFixtures/bin/Release/net10.0/DocuNet.TestFixtures.dll
Copy-Item ../output/demo/index.htm ../output/demo/index.html
```

Open output/demo/index.html. The extra copy only adapts the hosting entry point; the original output and its index.htm links are preserved.

## Future improvements

The demo reflects the current templates. Responsive design, navigation improvements and jQuery replacement will follow the [roadmap](roadmap.md), preserving content and link contracts.
