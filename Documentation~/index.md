# harookie Unity GameKit Documentation

This package provides small, reusable Unity gameplay systems. Each system is intentionally narrow: add only the assembly and namespace you need, wire the required scene references, and keep game-specific behavior in your own project code.

## System Docs

- [Core](core.md): scene loading, runtime registries, cutscene gating, and foldable inspector attributes.
- [Events](events.md): local event buses, global events, and typed payload values.
- [Interaction](interaction.md): reusable gameplay interfaces and save contracts.
- [Navigation](navigation.md): scene history, scene replacement, back navigation, and base scene controllers.
- [UI](ui.md): runtime UGUI panel, button, and text creation.
- [Dialogue](dialogue.md): dialogue labels, queued playback, voiced lines, timed voiced sequences, and TMP text animation.
- [Spatial](spatial.md): trigger volumes, spatial SFX, detection filters, runtime lookup, and save state.
- [Debug Tools](debug-tools.md): FPS/resolution/scene overlays and build version labels.
- [Editor](editor.md): foldable inspectors for MonoBehaviour components.
- [Changelog Maintenance](changelog-maintenance.md): how to keep release notes clean.

## Assembly Overview

The package is split into focused assemblies:

- `GameKit.Runtime`: original runtime helpers for Navigation and UI.
- `GameKit.Core`: core runtime helpers.
- `GameKit.Events`: event bus helpers.
- `GameKit.Interaction`: shared gameplay contracts.
- `GameKit.Dialogue`: dialogue components. Requires TextMeshPro.
- `GameKit.Spatial`: spatial trigger and SFX components. References `GameKit.Interaction`.
- `GameKit.DebugTools`: debug UI components. Requires TextMeshPro.
- `GameKit.Editor`: editor-only inspector tooling.

## General Usage Rules

- Prefer composition: attach GameKit components to scene objects and implement project-specific behavior in your own MonoBehaviours.
- Keep identifiers stable: any `uniqueId`, `SaveKey`, or asset reference used for save/load should not change after release.
- Wire serialized references explicitly in prefabs. Auto-lookup exists for convenience, but prefabs should be clear and deterministic.
- Treat static registries and global buses as process-wide state. Clear them when entering tests, changing profiles, or resetting game state.
- Keep package updates documented in `CHANGELOG.md` under `[Unreleased]`.
