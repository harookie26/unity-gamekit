# Events

Namespace: `GameKit.Events`

The Events system provides a small event bus for decoupled gameplay messages. Use it for simple in-process notifications, not as a replacement for a full save system, network message layer, or complex domain event framework.

## Types

- `EventBus`: instance-based event bus.
- `GlobalEventBus`: shared static `EventBus` instance.
- `EventPayload`: typed key/value payload container.

## Local EventBus

Use a local `EventBus` when a feature owns the event lifecycle.

```csharp
using GameKit.Events;
using UnityEngine;

public sealed class DoorEventsExample : MonoBehaviour
{
    private readonly EventBus events = new();

    private void OnEnable()
    {
        events.Add("door.opened", OnDoorOpened);
    }

    private void OnDisable()
    {
        events.Remove("door.opened", OnDoorOpened);
    }

    private void OnDoorOpened()
    {
        Debug.Log("Door opened.");
    }

    public void OpenDoor()
    {
        events.Publish("door.opened");
    }
}
```

## Payload Events

Use `EventPayload` for small, structured values.

```csharp
using GameKit.Events;
using UnityEngine;

public sealed class ScoreReporter : MonoBehaviour
{
    private readonly EventBus events = new();

    private void OnEnable()
    {
        events.Add("score.changed", OnScoreChanged);
    }

    private void OnDisable()
    {
        events.Remove("score.changed", OnScoreChanged);
    }

    private void OnScoreChanged(EventPayload payload)
    {
        int score = payload.Get("score", 0);
        Debug.Log(score);
    }

    public void PublishScore(int score)
    {
        EventPayload payload = new();
        payload.Set("score", score);
        events.Publish("score.changed", payload);
    }
}
```

## GlobalEventBus

Use `GlobalEventBus.Instance` for application-level signals that genuinely cross system boundaries.

```csharp
using GameKit.Events;
using UnityEngine;

public sealed class PauseListener : MonoBehaviour
{
    private void OnEnable()
    {
        GlobalEventBus.Instance.Add("game.paused", OnGamePaused);
    }

    private void OnDisable()
    {
        GlobalEventBus.Instance.Remove("game.paused", OnGamePaused);
    }

    private void OnGamePaused()
    {
        Time.timeScale = 0f;
    }
}
```

## Proper Use

- Always unsubscribe in `OnDisable` or `OnDestroy`.
- Use stable event names such as `game.paused`, `inventory.changed`, or `dialogue.completed`.
- Prefer local `EventBus` instances for feature-specific events.
- Reserve `GlobalEventBus` for cross-cutting application events.
- Call `GlobalEventBus.Clear()` when resetting runtime state or running isolated tests.
- Keep payload keys documented near the publisher. Payload values are type-checked at retrieval time.

## Common Mistakes

- Publishing before listeners subscribe. If ordering matters, use explicit initialization.
- Using global events for request/response flows. Prefer direct service calls for that.
- Passing large mutable objects in `EventPayload` without clear ownership.
