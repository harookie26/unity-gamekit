# Changelog

## [Unreleased]

### Added
- Added robust per-system documentation under `Documentation~`.
- Added core helpers for scene loading, runtime registries, cutscene gating, and foldable inspector attributes.
- Added event bus helpers with optional payload values.
- Added interaction and level contracts for reusable gameplay systems.
- Added TMP dialogue labels, queued dialogue playback, voiced dialogue entries, and timed voiced sequences.
- Added reusable non-voiced dialogue sequence assets and an importable Dialogue Starter sample.
- Added unified `DialogueAsset` playback and a reusable `DialoguePlayer` component.
- Added spatial triggers and spatial SFX components with saveable active state.
- Added debug overlay and build version label components.
- Added editor support for foldable component inspectors.

### Changed
- Split new runtime systems into focused assembly definitions.

### Fixed
- Guarded dialogue playback against missing label prefab setup.
- Made spatial SFX stop only after all matching targets exit the trigger.
- Added duplicate id diagnostics for spatial trigger and SFX registries.
- Added automatic TMP label lookup for build version labels.

## [0.1.1] - 2026-07-10

### Fixed
- Fixed Git package install URL.
