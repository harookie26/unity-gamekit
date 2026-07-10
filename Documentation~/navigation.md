# Navigation

Namespace: `GameKit.Navigation`

The Navigation system provides static scene navigation with simple back-history plus a base MonoBehaviour for scene controllers.

## Types

- `SceneNavigator`: static scene loading and history.
- `SceneControllerBase`: MonoBehaviour base class with Escape-key back handling and UnityEvent-friendly methods.

## SceneNavigator

```csharp
using GameKit.Navigation;

SceneNavigator.LoadScene("Inventory");
SceneNavigator.ReplaceScene("MainMenu");
SceneNavigator.Back("MainMenu");
SceneNavigator.ClearHistory();
```

### Behavior

- `LoadScene(sceneName)` pushes the current scene into history, then loads the target scene.
- `ReplaceScene(sceneName)` loads the target scene without adding history.
- `Back(fallbackScene)` loads the most recent distinct scene from history, or the fallback scene if history is empty.
- `ClearHistory()` clears the stack.

## SceneControllerBase

Inherit from `SceneControllerBase` for screens that need common scene button handlers.

```csharp
using GameKit.Navigation;

public sealed class OptionsController : SceneControllerBase
{
    public void OpenCredits()
    {
        LoadScene("Credits");
    }
}
```

Attach the controller to a scene object, then wire UI buttons to:

- `LoadScene(string sceneName)`
- `ReplaceScene(string sceneName)`
- `Back()`
- `ExitGame()`

## Proper Use

- Use `LoadScene` for screens the player should return from.
- Use `ReplaceScene` for hard transitions such as boot, main menu, or starting a new run.
- Set `backScene` in the inspector for each `SceneControllerBase`.
- Call `ClearHistory()` when starting a new game session or returning to the main menu.
- Ensure all target scenes are in Build Settings.

## Input System Requirement

`SceneControllerBase` uses `UnityEngine.InputSystem.Keyboard` to detect Escape. The package depends on the Unity Input System package.
