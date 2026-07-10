# harookie Unity GameKit

Reusable Unity gameplay infrastructure and UI helpers.

## Install

Use Unity Package Manager with a local path while developing:

```json
"com.harookie.unity-gamekit": "file:D:/GitHub/source/repos/unity-gamekit"
```

Use a Git URL once this repo is pushed:

```json
"com.harookie.unity-gamekit": "https://github.com/harookie26/unity-gamekit.git#v0.1.1"
```

## Namespaces

- `GameKit.Core`
- `GameKit.DebugTools`
- `GameKit.Dialogue`
- `GameKit.Events`
- `GameKit.Interaction`
- `GameKit.Navigation`
- `GameKit.Spatial`
- `GameKit.UI`

## Systems

- Core helpers: scene loading, runtime registries, cutscene gating, and foldable inspector attributes.
- Events: lightweight local and global event bus helpers with typed payload values.
- Interaction: shared contracts for interactables, collectibles, channeling, saveable objects, rooms, and stairs.
- Dialogue: TMP dialogue labels, queued dialogue playback, voiced dialogue entries, and timed voiced sequences.
- Spatial: trigger volumes, spatial SFX playback, target filtering, runtime lookup, and saveable active state.
- Debug tools: FPS/resolution/scene overlay and build version labels.

## Changelog Maintenance

- Add user-facing changes under `CHANGELOG.md` > `[Unreleased]` in the same commit as the code change.
- Use `Added`, `Changed`, `Fixed`, `Removed`, `Deprecated`, and `Security` sections only when they contain entries.
- When releasing, rename `[Unreleased]` to `[x.y.z] - YYYY-MM-DD`, bump `package.json`, tag the same version, then add a fresh empty `[Unreleased]`.
