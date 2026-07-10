# Debug Tools

Namespace: `GameKit.DebugTools`

Debug Tools provide small TMP-based UI components for runtime diagnostics.

Requires TextMeshPro.

## Types

- `PerformanceOverlay`: displays FPS, optional frame time, scene name, and resolution.
- `BuildVersionLabel`: displays a build version string.

## PerformanceOverlay Setup

1. Create a Canvas for debug UI.
2. Add TMP text objects for FPS, scene name, and resolution.
3. Add `PerformanceOverlay` to a controller object.
4. Assign:
   - `Fps Text`
   - `Scene Name Text`
   - `Resolution Text`
5. Set `Update Interval`, `Show Ms`, and `Visible On Awake`.

If `Fps Text` is not assigned, the component tries to find a child `TextMeshProUGUI`.

## Usage

```csharp
using GameKit.DebugTools;
using UnityEngine;

public sealed class DebugOverlayHotkey : MonoBehaviour
{
    [SerializeField] private PerformanceOverlay overlay;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
            overlay.Toggle();
    }
}
```

Call `RefreshStaticText()` if the active scene or resolution changes and you want the static labels updated immediately.

## BuildVersionLabel Setup

1. Add `BuildVersionLabel` to a TMP text object or parent object.
2. Assign `Label`, or let the component find a local/child `TextMeshProUGUI`.
3. Set `Build Version`.
4. Set `Visible On Awake`.

```csharp
using GameKit.DebugTools;
using UnityEngine;

public sealed class VersionBootstrap : MonoBehaviour
{
    [SerializeField] private BuildVersionLabel versionLabel;

    private void Start()
    {
        versionLabel.SetBuildVersion(Application.version);
    }
}
```

## Proper Use

- Keep debug UI under a dedicated Canvas so it can be excluded or hidden easily.
- Use `SetVisible(false)` for release builds if the objects remain in scenes.
- Use `Application.version`, build metadata, or CI-injected strings for version text.
- Avoid expensive diagnostic work in these components; they are intended for lightweight display only.
