# Interaction

Namespace: `GameKit.Interaction`

The Interaction system defines contracts shared by gameplay systems. These are interfaces only; your project implements the actual behavior.

## Interfaces

- `IInteractable`: object that can be interacted with.
- `ICollectible`: object that can be collected and has a stable id.
- `IChannelable`: object with start/stop channeling behavior.
- `ILinkable`: object that participates in id-based links.
- `IHidable`: hiding spot or hiding controller contract.
- `ISaveable`: object that can capture and restore state.
- `IRoom`: room data for level or navigation systems.
- `IStair`: connection data between rooms.

## IInteractable

```csharp
using GameKit.Interaction;
using UnityEngine;

public sealed class DoorInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        // Open, close, inspect, or trigger the door.
    }
}
```

Interaction code can then use `TryGetComponent<IInteractable>`.

```csharp
if (hit.collider.TryGetComponent(out IInteractable interactable))
    interactable.Interact();
```

## ICollectible

Use a stable `Id` for save files and inventory records.

```csharp
using GameKit.Interaction;
using UnityEngine;

public sealed class CoinPickup : MonoBehaviour, ICollectible
{
    [SerializeField] private string id;

    public string Id => id;

    public void Collect()
    {
        gameObject.SetActive(false);
    }
}
```

## IChannelable

Use for actions that have a held/start-stop lifecycle.

```csharp
using GameKit.Interaction;
using UnityEngine;

public sealed class RitualAltar : MonoBehaviour, IChannelable
{
    public void StartChannel()
    {
        // Start progress.
    }

    public void StopChannel()
    {
        // Pause, cancel, or resolve progress.
    }
}
```

## ISaveable

`ISaveable` defines a stable key and object state. The package does not provide a save manager; your project owns serialization and persistence.

```csharp
using GameKit.Interaction;
using UnityEngine;

public sealed class DoorState : MonoBehaviour, ISaveable
{
    [SerializeField] private string saveKey;
    [SerializeField] private bool isOpen;

    public string SaveKey => saveKey;

    public object CaptureState()
    {
        return isOpen;
    }

    public void RestoreState(object state)
    {
        if (state is bool open)
            isOpen = open;
    }
}
```

## Level Contracts

`IRoom` and `IStair` are lightweight contracts for level graph systems.

```csharp
using GameKit.Interaction;
using UnityEngine;

public sealed class RoomNode : MonoBehaviour, IRoom
{
    [SerializeField] private int id;
    [SerializeField] private Bounds bounds;

    public int Id => id;
    public Vector3 Center => bounds.center;
    public Bounds Bounds => bounds;
}
```

## Proper Use

- Keep ids stable after shipping content.
- Do not put heavy behavior in interface implementations just to satisfy a generic system.
- Use explicit save DTOs for complex state instead of anonymous dictionaries.
- Validate duplicate ids in editor tooling or scene tests.
- Prefer `TryGetComponent` over broad scene searches during gameplay.
