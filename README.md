# harookie Unity GameKit

Reusable Unity gameplay infrastructure and UI helpers.

## Install

Install through Unity Package Manager using the Git URL:

1. In Unity, open `Window > Package Manager`.
2. Click `+`.
3. Select `Add package from git URL...`.
4. Enter:

```text
https://github.com/harookie26/unity-gamekit.git#v0.1.1
```

To install through `Packages/manifest.json`, add:

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

## Documentation

Full system docs live in [`Documentation~/index.md`](Documentation~/index.md).

- [`Core`](Documentation~/core.md)
- [`Events`](Documentation~/events.md)
- [`Interaction`](Documentation~/interaction.md)
- [`Navigation`](Documentation~/navigation.md)
- [`UI`](Documentation~/ui.md)
- [`Dialogue`](Documentation~/dialogue.md)
- [`Spatial`](Documentation~/spatial.md)
- [`Debug Tools`](Documentation~/debug-tools.md)
- [`Editor`](Documentation~/editor.md)
- [`Changelog Maintenance`](Documentation~/changelog-maintenance.md)

## Changelog Maintenance

- Add user-facing changes under `CHANGELOG.md` > `[Unreleased]` in the same commit as the code change.
- Use `Added`, `Changed`, `Fixed`, `Removed`, `Deprecated`, and `Security` sections only when they contain entries.
- When releasing, rename `[Unreleased]` to `[x.y.z] - YYYY-MM-DD`, bump `package.json`, tag the same version, then add a fresh empty `[Unreleased]`.
