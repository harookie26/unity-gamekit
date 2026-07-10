# Core

Namespace: `GameKit.Core`

The Core system contains small runtime utilities that are shared by other gameplay systems or useful across scenes.

## Components and Types

- `SceneLoader`: MonoBehaviour wrapper around `SceneManager` loading and application quit.
- `RuntimeRegistry<T>`: simple in-memory registry for unique class instances.
- `CutsceneGate`: static counter for blocking input/gameplay while one or more cutscenes are active.
- `FoldableInspectorAttribute`: marker attribute used by the editor foldable inspector.

## SceneLoader

Attach `SceneLoader` to a scene object when you want UnityEvents, buttons, timeline signals, or animation events to load scenes without writing a custom wrapper.

### Setup

1. Add a GameObject such as `Scene Services`.
2. Add `SceneLoader`.
3. Ensure target scenes are listed in Build Settings.
4. Hook UI buttons or UnityEvents to one of the public methods.

### Public Methods

```csharp
public void LoadSceneByName(string sceneName)
public void LoadSceneByIndex(int sceneIndex)
public void LoadSceneAdditive(string sceneName)
public void QuitGame()
```

### Example

```csharp
using GameKit.Core;
using UnityEngine;

public sealed class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    public void Play()
    {
        sceneLoader.LoadSceneByName("Level01");
    }
}
```

### Proper Use

- Use `LoadSceneByName` for readable menu and level flows.
- Use `LoadSceneByIndex` only when build index order is part of your release process.
- Use `LoadSceneAdditive` for additive content, overlays, or streamed sections.
- Do not pass empty scene names. The component logs an error and ignores the call.

## RuntimeRegistry<T>

Use `RuntimeRegistry<T>` when a manager needs to track active objects without making each object globally static.

```csharp
using GameKit.Core;

public sealed class EnemyRegistry
{
    private readonly RuntimeRegistry<Enemy> enemies = new();

    public IReadOnlyList<Enemy> Enemies => enemies.Items;

    public void Register(Enemy enemy) => enemies.Register(enemy);
    public void Unregister(Enemy enemy) => enemies.Unregister(enemy);
}
```

### Proper Use

- Register in `OnEnable` and unregister in `OnDisable` for scene objects.
- Call `Clear()` when resetting a runtime session.
- The registry prevents duplicate references, but it does not own object lifetime.

## CutsceneGate

`CutsceneGate` is a static counter. Each caller that begins a blocking sequence calls `Begin()`, and the same caller must later call `End()`.

```csharp
using GameKit.Core;
using UnityEngine;

public sealed class PlayerInputBlocker : MonoBehaviour
{
    private void Update()
    {
        if (CutsceneGate.IsActive)
            return;

        // Read player input here.
    }
}
```

### Proper Use

- Pair every `Begin()` with `End()`.
- Use `Reset()` when restarting a run, loading a fresh profile, or entering tests.
- `Begin()` returns `true` only when the gate transitions from inactive to active.
- `End()` returns `true` when all active gates have ended.

## FoldableInspectorAttribute

Add this attribute to a MonoBehaviour to opt into the foldable editor inspector.

```csharp
using GameKit.Core;
using UnityEngine;

[FoldableInspector(displayName: "Door Controller")]
public sealed class DoorController : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool startsOpen;

    [Header("Audio")]
    [SerializeField] private AudioClip openClip;
}
```

See [Editor](editor.md) for foldable inspector behavior.
