# HARNESS protocol

## Scope and required reading

This is the AI workflow protocol for DocuNet, adapted to this repository's agreements. Before implementing, read this guide, [the Git workflow](git-workflow.md) and [the requirement template](requirement-template.md). docs/ contains application documentation; docs/harness/ contains harness rules; docs/requirements/ contains requirement history.

The roles are Coordinator, Developer and Tester. They may be performed by separate agents or the same agent in explicit phases. Role separation does not imply an independent review when a single agent performs all roles; record that limitation. The Coordinator remains responsible for the workflow and consolidates deliveries.

## States and approvals

Draft → Awaiting scope approval → In development → In validation → Awaiting final approval → Approved → Closed.

For requirements involving discovery, insert Research → Awaiting behavior/design approval after scope approval and before development. A failure returns the work to the appropriate phase. Record blockers without declaring the requirement approved or closed.

1. **Start — Coordinator.** Check that the working tree is clean and synchronize according to the Git workflow. If a remote, commits or permissions are missing, record the limitation; do not simulate a successful pull. Create the record using the template. Filename: YYYY-MM-DD_CODE_slug.md, for example 2026-10-15_DOCUNET-001_generate-versioned-documentation.md. The date marks the start of the solution, not closure; use the user's local date. The code is unique and remains stable.
2. **Definition — Coordinator.** Review the objective, description, scope and exclusions, verifiable criteria, traceability, unit/regression/integration test design, branches and approvals. Link criteria to concrete tests. Resolve ambiguities affecting behavior or acceptance. Explain why any section is not applicable.
3. **Human scope approval.** Present the record and request approval before coding. Record the actual decision and its conversation or review reference; do not infer approval from silence. Substantial scope changes require renewed approval. This gate does not prevent preparation of requested documentation and analysis.
4. **Implementation — Developer.** Work on the agreed branch. Implement only the approved scope, write unit tests and run them. Deliver changed files, the criterion/change/test mapping, commands and results, limitations and risks to the Coordinator. Do not declare human acceptance.
5. **Review — Coordinator.** Compare the delivery against each criterion and review code and tests. Return noncompliant items to the Developer. Update usage, architecture and cross-repository contract documentation where appropriate; hand the Tester an identifiable revision and test plan.
6. **Validation — Tester.** Run the defined unit, regression and integration tests, using inputs from tests/fixtures/ and output under `tests/results/<code>/<run>/`. Record the tested revision, environment, commands, exit codes, expected/actual results and evidence paths. When scope affects publishing, test integration against a temporary repository and preservation of existing versions. Check links when generating HTML. Do not use the published site as a fixture. Report failures or skipped tests and return control to the Coordinator; an unexecuted test does not count as successful.
7. **Human result approval.** The Coordinator presents criteria, changes, evidence and limitations. Request final approval and, where appropriate, explicit authorization to merge into master. Keep the work pending if there is no response or approval is rejected.
8. **Closure — Coordinator.** After approval, complete traceability, commits and the authorized merge according to the Git workflow. Check that the working tree is clean. Record outstanding work if closure cannot be completed; never delete changes to appear clean.

## Evidence and traceability

The record must connect the requirement code, branch, criteria, tests, implementation commits, documentation and human approval. Use the code in commits and PRs. Record the author/role, date and reason for decisions. Do not invent hashes, executions or approvals.

Large results remain ignored; preserve a reviewable summary in the record or linked documentation. Before closure, check that referenced evidence is available in the repository or an accessible persistent artifact. The closing commit contains the record and is identified by the code; do not try to include its own hash inside the file.

## Refinement, research and session handoff

Distinguish **refinement** (analyzing and editing documents) from **execution** (investigating artifacts, coding or testing the solution). A request to prepare a requirement does not authorize execution. During refinement, preserve the original intent, turn ambiguous terms into verifiable criteria and record decisions, proposals and pending items separately. If the human excludes this documentation work from the protocol, respect that exception without extending it to future sessions or requirements.

For requirements involving technical discovery, define phases: research, approval of recovered behavior/design and implementation. The Coordinator may conduct research with specialist support and deliver evidence to the human before delegating coding to the Developer. Do not force unit tests for a documentation-only deliverable: use appropriate reviews and evidence, explaining what is not applicable.

Do not require behaviors that research is meant to discover to be known before research starts. Instead, define questions, artifacts, deliverables and conditions for approving the next phase. Separate observations, inferences and unknowns; changes to the approved contract require human review.

Each record must include a handoff: actual state, confirmed decisions, open questions, local inputs ignored by Git, paths and hashes when available, executed versus proposed commands, persistent deliverables and the next authorized action. Do not treat a human clarification as global approval.

Validation adapts to scope: an isolated generator does not require testing Git in the published site's repository; a documentation change does not require building the application. Test cross-repository integration and version preservation when the requirement affects them. An unavailable platform or fixture remains pending, not a successful test.
