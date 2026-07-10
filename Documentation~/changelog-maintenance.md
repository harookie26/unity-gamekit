# Changelog Maintenance

This package uses a simple Keep a Changelog-style file at `CHANGELOG.md`.

## Daily Workflow

When a change affects users, add it under `[Unreleased]` in the same commit as the code or docs change.

Use these sections only when they contain entries:

- `Added`: new features, systems, public APIs, docs, or examples.
- `Changed`: behavior changes, API changes, workflow changes.
- `Fixed`: bugs, crashes, incorrect behavior, missing references.
- `Removed`: deleted features or APIs.
- `Deprecated`: features still present but planned for removal.
- `Security`: security-sensitive fixes.

## Entry Style

Use short, user-facing sentences.

Good:

```md
- Added spatial SFX playback for trigger-based ambient audio.
- Fixed dialogue playback when the label prefab is missing.
```

Avoid:

```md
- Changed some files.
- Refactored stuff.
- WIP.
```

## Release Workflow

1. Make sure `[Unreleased]` contains every user-facing change.
2. Bump `package.json` to the release version.
3. Rename `[Unreleased]` to `[x.y.z] - YYYY-MM-DD`.
4. Add a fresh empty `[Unreleased]` section at the top.
5. Commit the release.
6. Tag the commit with the same version, such as `v0.1.2`.

## Versioning Guidance

Before `1.0.0`, use pragmatic package versioning:

- Patch: bug fixes and docs that do not change behavior.
- Minor: new systems, new public APIs, or compatible behavior additions.
- Major: reserved for stable post-`1.0.0` breaking changes.

For this package, adding a new runtime system should usually be a minor version bump.
