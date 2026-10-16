# Testing and coverage

Tests use xUnit.net and NSubstitute. The normal suite builds its own fixtures and requires no private samples. Test projects and coverage configuration are inside src.

## Run the suite

From src:

```shell
dotnet restore --locked-mode
dotnet build
dotnet test --no-build
```

## Measure coverage

From src, using PowerShell 7 to check thresholds:

```powershell
dotnet test --no-build --coverage --coverage-settings coverage.config --coverage-output-format cobertura --coverage-output coverage.cobertura.xml --results-directory ../tests/results/coverage
./Check-Coverage.ps1 -Report ../tests/results/coverage/coverage.cobertura.xml
```

For Release, add --configuration Release to both build and test.

The relative --coverage-output name is resolved under the results directory. Configuration includes only DocuNet.Core, DocuNet.Rendering and DocuNet.Cli; it excludes tests, fixtures, dependencies and C# generated dynamically by Spark. Production source files are not excluded to improve percentages.

Microsoft.Testing.Extensions.CodeCoverage integrates measurement with Microsoft.Testing.Platform. See the [official documentation](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-code-coverage).

Check-Coverage.ps1 requires at least **90% line coverage and 80% branch coverage in each project** and fails if any of the three projects is missing. Coverage measures execution: it must be accompanied by assertions about results, errors and data preservation.

## Protected behaviors

| Area | Main cases |
| --- | --- |
| Metadata | Reading without executing modules/attributes; visibility; identity by declaring class; generics; inheritance; Framework/modern references; enums; events; nested types; global namespace; duplicates and missing dependencies |
| XML | Exact IDs; ref/out, indexers and arrays; missing comments; invalid structure; unnamed members, duplicates and rejected DTDs |
| HTML | Escaping; paragraphs, lists and references; overloads with unique anchors; links and fragments |
| Templates | Name expansion; partials; editing without recompiling; resources; failures without replacing the previous site |
| Files | Deterministic regeneration; unrelated file preservation; collisions, manual edits, traversal, overlap and filesystem root |
| CLI | Help; arguments; patterns; deduplication; companion XML; diagnostics and exit codes |

PresentationContractTests checks content through the DOM and link targets. It does not freeze complete HTML or CSS, allowing presentation to evolve.

SampleValidationTests is an explicit test for comparing additional sites and inputs through environment variables. Its omission from the normal suite is deliberate. The public suite must remain reproducible without those files.

## Add regression tests

Check the observable result that distinguishes correct behavior from the defect. Use synthetic files and assemblies to test real processing; reserve NSubstitute for simulated collaborators. Do not replace assertions with superficial execution or hide code to meet thresholds.

New templates also require viewport, keyboard, accessibility and browser navigation tests. Store local reports under tests/results, which Git ignores.

The CI matrix includes Windows and Linux. Additional platform validation belongs to the distribution work in the [roadmap](roadmap.md#distribution-by-platform-and-architecture).
