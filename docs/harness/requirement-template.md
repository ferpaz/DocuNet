# Requirement template

Copy to docs/requirements/YYYY-MM-DD_CODE_slug.md. Complete before coding; explain every "Not applicable" entry.

# CODE — Title

- Start: YYYY-MM-DD (user's time zone)
- Status: Draft
- Coordinator / Developer / Tester:
- Mode: separate agents or phases of the same agent
- Base / development branch / target:
- Affected repositories:

## Objective and description

Problem, beneficiary, expected result, inputs and outputs. Include the library or product version when applicable.

## Scope

Included changes, exclusions, dependencies, risks and open questions.

## Inputs, phases and decisions

List artifacts and paths, availability, sensitivity and whether Git ignores them. Record identity/hashes when inspected. Distinguish confirmed decisions, proposals and open questions.

If research is needed, define questions and deliverables, persistent evidence and human approval of results before implementation. Specify observable equivalence and permitted differences when replacing an existing tool.

## Acceptance criteria

| ID | Scenario and verifiable result | Associated tests |
| --- | --- | --- |
| CA-01 | To be completed | UT-01, IT-01 |

## Test design

| ID | Type (unit/regression/integration) | Input or fixture | Command/procedure | Expected result |
| --- | --- | --- | --- | --- |
| UT-01 | Unit | To be completed | To be completed | To be completed |
| IT-01 | Integration | To be completed | To be completed | To be completed |

Define regression coverage, platforms to validate, environment setup/cleanup and result locations. Include negative cases and preservation of existing versions where applicable. Do not record proposed commands as executed.

## Human approvals

| Gate | Status | Person, date and reference | Conditions |
| --- | --- | --- | --- |
| Scope before execution | Pending | — | — |
| Behavior/design after research, if applicable | Pending | — | Before coding |
| Validated result | Pending | — | — |
| Merge into target | Pending | — | — |
| Push/deployment, if applicable | Pending | — | — |

## Developer delivery and Coordinator review

Files, decisions, satisfied criteria, executed unit tests, updated documentation and outstanding items.

## Tester report

Tested revision, environment, commands actually executed, results and persistent evidence. List failures and omissions. State who receives control next.

## Traceability and closure

Implementation commits, documentation, PR, merge, validated integrated revision and final state of each repository. Record bootstrap exceptions, conflicts and human decisions. Close only after approval and a clean working tree check.

## Session handoff

Actual state, confirmed decisions, blockers, inputs the human must supply, documents/evidence to read and next authorized action. Identify executed, proposed and pending tests. Do not assume local data is available just because it existed in another session.
