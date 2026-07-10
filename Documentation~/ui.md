# UI

Namespace: `GameKit.UI`

`RuntimeUiFactory` creates simple UGUI panels, buttons, and text at runtime. It is useful for prototypes, debug menus, generated overlays, and lightweight runtime screens.

## Setup

1. Create or reuse a `Canvas`.
2. Provide a Unity `Font`.
3. Create `RuntimeUiFactory`.
4. Add controls to a parent `RectTransform`.

## Example

```csharp
using GameKit.UI;
using UnityEngine;
using UnityEngine.UI;

public sealed class RuntimeMenu : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private Font font;

    private void Start()
    {
        RuntimeUiFactory ui = new(font);

        ui.Panel(
            root,
            "Background",
            RuntimeUiFactory.R(24, 24, 320, 180),
            new Color(0f, 0f, 0f, 0.65f));

        Button button = ui.Button(
            root,
            "StartButton",
            "Start",
            RuntimeUiFactory.R(48, 64, 160, 42),
            new Color(0.18f, 0.28f, 0.24f, 1f),
            Color.white,
            18);

        button.onClick.AddListener(OnStart);
    }

    private void OnStart()
    {
        Debug.Log("Start clicked.");
    }
}
```

## Coordinates

`RuntimeUiFactory.R(x, y, width, height)` creates a `Rect` using top-left positioning. Internally, generated `RectTransform`s are anchored to the top-left of the parent.

## Custom Colors

```csharp
RuntimeUiFactory ui = new(
    font,
    outlineColor: new Color(0f, 0f, 0f, 0.35f),
    highlightAccent: Color.yellow,
    pressedAccent: Color.green);
```

## Proper Use

- Use this for runtime-generated UI, debug UI, and quick internal tools.
- Prefer authored prefabs for production UI screens with complex layout, animation, localization, or accessibility needs.
- Pass a valid `Font`; generated `Text` components require it.
- Keep generated element names stable if other code needs to find them.
- Parent generated controls under an existing Canvas hierarchy.
