# Test inputs

Buildable fixtures are in src/tests/DocuNet.TestFixtures and xUnit tests in src/tests/DocuNet.Tests. This directory accepts minimal additional data per scenario; it does not contain projects or private assemblies.

Include the source, preparation steps and consuming tests when adding a dataset. Prefer buildable source over DLL/PDB files. Do not modify original inputs during tests; use temporary directories.

Private samples remain under samples, which is ignored. Local output is stored under `tests/results/<code>/<run>/`, also ignored. See [procedure and coverage](../../docs/testing.md).
