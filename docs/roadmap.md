# Roadmap

These items describe proposed improvements. They do not represent available features or committed delivery dates.

## Priority and execution order

Prioritize CLI functionality and the usefulness of the generated site. Address distribution and installation after stabilizing options, templates and resource management.

| Order | Issues | Expected outcome and dependencies |
| --- | --- | --- |
| 1 | CLI parameter and help modernization | Improve CLI usability and prepare consistent options for visual frameworks and resources. |
| 2 | CSS and JavaScript dependencies: CDN or local files | Define how to declare, resolve and publish resources for the new templates. |
| 3 | Presentation and navigation; Bootstrap and Tailwind CSS; JavaScript and legacy resource modernization | Coordinate these issues to modernize the documentation experience and avoid reworking navigation and styles. |
| 4 | Custom template management | Define customization, compatibility and preservation during upgrades based on the new templates. |
| 5 | Distribution by platform and architecture, including macOS | Package and validate the tool with established options and resource structure. |
| 6 | Windows installation through WinGet | Use the Windows packages and previously defined update and file preservation policies. |

Define the desired design and navigation when roadmap execution begins, even though implementation belongs to phase 3. This initial design will guide resource decisions in phase 2.

Deliver visual modernization incrementally: agree on shared navigation, implement one template variant and then the second. Phase 3 is complete when users can select either Bootstrap or Tailwind CSS with the same features and content, and legacy JavaScript has been modernized according to the design.

Create follow-up issues for Bootstrap, Tailwind CSS and dependency updates when integration is complete. Review them for every future CLI release.

## CLI parameter and help modernization

Modernize parameter presentation, help and argument processing using maintained NuGet libraries compatible with .NET 10. Select libraries during issue analysis, considering maintenance, licensing, dependencies and compatibility with distribution by platform and architecture.

Proposed scope:

- Replace manual argument processing with a library that consistently declares options, types, defaults, required parameters and validation rules.
- Generate help from parameter definitions to keep names, descriptions, allowed values and usage examples synchronized. Improve --help readability and the presentation of options and errors.
- Provide clear diagnostics for unknown options, missing or invalid values and incompatible combinations, with stable exit codes for automation.
- Evaluate a console presentation library to improve formatting, adapt to terminal width and provide readable output without colors or control sequences when redirected to files or run in CI.
- Preserve compatibility with existing commands and parameters; document any necessary changes and migration steps for user scripts.
- Cover processing, validation, help and exit codes with xUnit tests, including quoted arguments, paths with spaces and execution on supported platforms. Use NSubstitute when mocks are needed.
- Update documentation and usage examples to match the final CLI contract.

## CSS and JavaScript dependencies: CDN or local files

Allow templates to use CSS and JavaScript dependencies from their projects' official or recommended CDNs, or copy their files into the generated site. Define how to select the resource mode during generation and declare it in templates.

Proposed scope:

- Offer a CDN mode with HTTPS references to explicit dependency versions, and a local mode with files copied into the site and relative links for static publishing and offline browsing.
- Identify official or recommended CDNs for each dependency; where no suitable source exists, retain the local alternative. Avoid references to floating versions.
- Keep CSS and JavaScript versions equivalent between modes and account for required related resources, such as fonts and images.
- Evaluate subresource integrity (SRI), crossorigin settings and CSP compatibility for external references, as supported by each provider.
- Define the default mode and document connection requirements for both generation and browsing. Determine whether local resources ship with templates or are obtained during generation.
- Preserve the corresponding third-party licenses and notices in both modes, and copy only resources required by the selected mode.
- Verify repeatable generation, links and navigation in both modes; check offline operation in local mode and document CDN mode behavior when its resources are unavailable.

Coordinate this issue with JavaScript and legacy resource modernization and custom template management.

## Presentation and navigation

Evolve external templates toward a modern, responsive presentation for developers browsing an API:

- Comfortable reading on desktop, tablet and mobile, including long signatures and code examples.
- Navigation by namespace, type and member, with location context and direct links to overloads.
- Search and filtering within the static site, preserving context during navigation.
- Keyboard access, visible focus, contrast and semantic structure.
- Legacy resource modernization according to the design.
- Visual, viewport and accessibility tests.

Keep the documentation model separate from presentation. The new design must preserve content, identifiers and link targets without being tied to snapshots of current templates.

## Bootstrap and Tailwind CSS in standard templates

Support Bootstrap and Tailwind CSS through standard template variants to improve visual appearance, consistency and adaptation to different screen sizes. Users must be able to choose which framework to use when generating the site; the CLI must support both and document the selection option and its default value.

Proposed scope:

- Use the latest stable version of each framework available when implementing and completing this requirement. Verify versions against official sources and record the specific versions supported by the CLI release.
- Modernize the visual structure of namespace, type and member pages in both variants, with consistent styles for navigation, tables, signatures, code examples and search, preserving the same features and content.
- Integrate Bootstrap components and Tailwind CSS utility styles, together with any additional components required by each variant; define their CSS and JavaScript dependencies and publish only the selected variant's resources.
- Keep templates external and editable, and document framework selection and style/component customization without modifying the executable.
- Define how to generate and distribute each variant's final styles. If resource compilation is required, evaluate doing it when preparing standard templates and document the process for users customizing them.
- Coordinate resource delivery with the CDN or local files issue, according to each framework's capabilities, and avoid including unused dependencies or styles.
- Preserve content, member identifiers, relative links and site navigation; validate both variants on desktop, tablet and mobile, including keyboard use and accessibility.
- Add examples and visual checks for both variants to review results and detect regressions in standard templates.
- Create follow-up issues to keep Bootstrap, Tailwind CSS and their dependencies updated in future CLI releases. Each update must review available stable versions, compatibility and custom template migrations, licenses, and visual and functional regressions; document the versions adopted in each release.

This issue is part of presentation and navigation modernization and must be coordinated with legacy JavaScript resource modernization.

## JavaScript and legacy resource modernization

Current templates include jQuery 1.3.2 (with Sizzle) and jQuery.ScrollTo 1.4.2. They are retained for compatibility; their update is not implemented or presented as resolved.

As part of modernization:

- Evaluate replacing used functionality with native browser APIs or maintained dependencies.
- Review security advisories and remove unnecessary libraries or functions.
- Preserve navigation, scrolling, code examples and direct links.
- Test screen sizes, keyboard use, focus, history and operation without a backend.
- Update notices/licenses and validate generated sites before removing current resources.

Design and JavaScript modernization must be treated as an improvement requiring behavioral regression checks; it will not be performed as an automatic package update.

## Custom template management

Define in a separate issue how to manage user-modified or user-created templates during CLI installation, upgrade and uninstallation, for all distribution methods.

Open decisions:

- Determine where to store custom templates and how to distinguish them from distributed templates, preserving selection through --templates.
- Define how to incorporate default template improvements without overwriting customizations and how to report version incompatibilities.
- Establish what is preserved during uninstallation and how users can explicitly delete their customizations.
- Document the agreed workflow and verify that upgrades preserve user changes.

## Distribution by platform and architecture

Evaluate separate packages or installers for Windows and Linux on x64 and ARM64. This work will also introduce macOS support on Intel and Apple Silicon, with its own publishing and validation. Each operating system and architecture combination requires a specific artifact.

| Proposed variant | Runtime identifier |
| --- | --- |
| Windows x64 | win-x64 |
| Linux x64 | linux-x64 |
| Windows ARM64 | win-arm64 |
| Linux ARM64 | linux-arm64 |
| macOS Intel | osx-x64 |
| macOS Apple Silicon | osx-arm64 |

Keep a single solution and codebase; vary publishing, packaging and testing by target.

Address macOS support within this distribution initiative. Its implementation, validation and CI integration are neither a prerequisite nor a requirement for earlier roadmap improvements. Until this work is complete, validated support is limited to Windows and Linux.

Requirements and options to evaluate:

- **Always include .NET Framework 4.8 references** under ReferenceAssemblies/net48 for every target, including Linux, Windows ARM64 and macOS. They allow Framework libraries to be interpreted without executing their assemblies and must not be confused with the CLI runtime.
- **Keep Templates as an external, editable directory**, with its templates and resources, to customize presentation without changing or recompiling the executable. Retain --templates to select another location.
- **Evaluate bundling referenced runtime DLLs** through single-file publishing. This packages dependencies rather than merging their assemblies; compatibility must be verified before adoption.
- **Prefer distribution without the .NET 10 runtime included, if feasible**: framework-dependent publishing, SelfContained=false. This requires the appropriate .NET 10 runtime to be installed for the system and architecture; using the CLI does not require the SDK.
- Pay particular attention to Spark/Roslyn, dynamic template compilation and metadata reading. If they require physical references, define which files must remain external or be extracted; do not assume enabling single-file is sufficient.
- Preserve third-party licenses and notices in every package, and verify paths, permissions, offline operation and generation with Framework and modern inputs.
- Test the published executable on each target and automate downloadable artifacts and their checks in CI. Do not count cross-compilation as validated execution.
- Implement and validate macOS within this work: restore, build, tests, coverage and generation using project-owned fixtures on Intel and Apple Silicon. Check paths, case sensitivity, permissions, resources, links and net48 references; record OS version, architecture and SDK/runtime. Add CI jobs and announce support only after corresponding validation.

The evaluation must compare size, installation requirements and user experience. Dependency bundling and runtime exclusion are compatibility-dependent goals; net48 references and external Templates customization are retained.

Reference: [.NET single-file publishing](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview).

## Windows installation through WinGet

Prepare distribution so DocuNet can be installed through WinGet on Windows x64 and ARM64:

- Publish the packages and manifests needed for inclusion in the WinGet catalog, with a stable identifier, versions, download URLs, hashes and license metadata.
- Support per-user installation (`winget install --id <package-identifier> --exact --scope user`) without administrator permissions. Define the identifier when preparing publication.
- Register the docunet command in the user's PATH, directly or through the appropriate WinGet package alias mechanism, so it can run from any directory. Preserve existing PATH entries and document when a new terminal must be opened.
- Install the complete package, including ReferenceAssemblies/net48 references, the external Templates directory and licenses. Resolve their paths independently of the user's working directory.
- Define and verify how to satisfy the .NET 10 runtime requirement if distribution remains framework-dependent, without bundling the runtime or assuming prerequisite installation always supports user scope.
- Validate manifests with winget validate and test installation, upgrade and uninstallation under unprivileged accounts for both architectures. Check the command from PowerShell and CMD in directories other than the installation directory, including paths with spaces.
- Ensure upgrade and uninstallation manage only the package's own PATH entries, aliases and files. Define the custom template policy in the [Custom template management](#custom-template-management) issue.

References: [WinGet installation and scope selection](https://learn.microsoft.com/en-us/windows/package-manager/winget/install) and [package manifests](https://learn.microsoft.com/en-us/windows/package-manager/package/manifest).
