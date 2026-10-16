# HARNESS Git workflow

## Start clean and up to date

1. Confirm the root, branch, remotes and upstream: `git rev-parse --show-toplevel`, `git status --short --branch`, `git remote -v`, `git branch -vv`.
2. If prior changes exist, identify their origin and stop modifying the requirement. Ask the human to decide how to preserve them. Do not automatically stash, reset, clean, discard or commit someone else's work.
3. With a clean working tree and configured upstream, run `git pull --ff-only`. On divergence, permission or authentication errors, report the blocker; do not force synchronization.
4. In a repository without commits or a remote, document the bootstrap exception. Documentation preparation may continue; agree on the Git base and configuration before implementing. Do not invent a remote URL.

## Working branch

Record the base and branch in the requirement record. Use `feature/<code>-<slug>` or `fix/<code>-<slug>` in lowercase. master is the intended destination; if it does not exist or the repository uses another destination, agree on the target before integrating.

After scope approval, create the branch from the synchronized base. Keep the prepared record alongside the requirement's work. Use focused commits such as "DOCUNET-001: Add version catalog". Review the diff and add explicit files; avoid including unrelated changes or secrets.

## Review and merge

Result approval and merge permission are recorded as separate decisions, although the human may grant both together. Push and deployment require explicit authorization; they are not implied by local merge permission.

Before merging, ensure complete commits and a clean working tree. Update the target with `git pull --ff-only`; incorporate the updated base into the working branch and revalidate if code or tested conditions changed. Do not rewrite shared history or force push. Respect PR protection if present.

Resolve conflicts only when intent is unambiguous. If they affect scope or behavior, request human help, identifying files, alternatives and impact. Do not automatically choose an entire version of a file. After resolving conflicts, run affected tests and record results. A substantial change after approval requires renewed approval.

With authorization, merge into the target. Verify that it contains the requirement's commits (`git merge-base --is-ancestor <commit> master`), review the result and run the agreed validation on the integrated revision. If it fails, report the failure and keep the requirement open; do not announce success or revert shared history without authorization.

## Closure

Update the record with approval, tested revisions, commits, PR or merge and results. If this record is added after merging, use a traceable documentation commit that follows target protections. Check `git diff --check` and `git status --porcelain`: the latter must be empty. Separately verify any modified publishing repository. A clean local tree does not imply push or deployment.

## Record prepared before branch creation

Refining a requirement may leave modified documents before execution starts. Do not attempt a pull with those changes or automatically classify them as someone else's work. Identify their origin and agree on preservation: for example, an authorized documentation commit before synchronization, or a clean base prepared by the human. Then synchronize and create the implementation branch. Do not automatically discard or stash the record to satisfy a clean start.
