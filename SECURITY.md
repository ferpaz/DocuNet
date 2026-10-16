# Security Policy

## Supported versions

Reports are evaluated against the latest revision of master. The project does not currently maintain LTS branches or guarantee backports for older versions.

## Reporting a vulnerability

Do not publish exploitable details, credentials, private DLLs or sensitive proof-of-concept material in public issues or pull requests.

If GitHub private vulnerability reporting is enabled for the repository, use Security → Report a vulnerability. Otherwise, contact the maintainer at [ferpaz64@gmail.com](mailto:ferpaz64@gmail.com) with the subject "DocuNet: security report". Use this first contact to agree on how to share sensitive material; do not attach confidential assemblies.

Include:

- Affected revision or version, operating system and SDK/runtime.
- Component and command involved.
- Minimal reproduction steps using synthetic data.
- Impact and required conditions, distinguishing observations from assumptions.
- Whether the issue affects the CLI, templates or generated site.

Reports are reviewed as availability allows; no fixed response time is promised. The maintainer will coordinate reproduction, remediation and disclosure with the reporter. Bugs without a security impact can be filed in GitHub Issues.

## Trust boundaries

DocuNet reads metadata without executing code or attribute constructors from the documented library. This is not a sandbox for malformed inputs: restrict permissions and resources when processing files from unknown sources.

Spark templates compile and execute C# supplied by their author; use only trusted templates. Do not accept arbitrary user templates in a public service without external isolation.

XML content is escaped in the supplied templates. Customizations must preserve this escaping and check links and paths. Keep the output manifest to protect managed files and unrelated content.

The current templates include legacy jQuery and ScrollTo. Their replacement is listed in the [roadmap](docs/roadmap.md); they are not claimed to be modern or free of vulnerabilities. Report any reproducible impact on DocuNet through the private channel above.

## Publishing and dependencies

Preserve third-party notices and licenses in distributions. Review dependency alerts and security changes before publishing releases. This policy does not itself enable GitHub private reporting or other repository security settings.
