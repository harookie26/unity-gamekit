# Spatial

Namespace: `GameKit.Spatial`

The Spatial system provides trigger-volume gameplay helpers and 3D SFX playback. It uses `DetectionTarget` filters, static id lookup, and `ISaveable` state for active/inactive status.

## Types

- `DetectionTarget`: layer and optional tag filter.
- `SpatialTrigger`: trigger volume that calls attached `SpatialTriggerAction`s.
- `SpatialTriggerAction`: base class for custom enter/exit behavior.
- `SpatialTriggerState`: serializable active-state DTO.
- `SpatialSfx`: trigger-volume 3D audio playback.
- `SpatialSfxState`: serializable active-state DTO.

## DetectionTarget

`DetectionTarget` matches a collider by layer mask and optional tag.

- Empty `Required Tag`: any tag is accepted.
- `Layer Mask`: collider layer must be included.
- Both checks must pass.

## SpatialTrigger Setup

1. Add `SpatialTrigger` to a GameObject.
2. A `SphereCollider` is required and configured as a trigger at runtime.
3. Set `Unique Id` if save/load or static lookup is needed.
4. Configure `Target` layer/tag.
5. Set `Trigger Radius`.
6. Add one or more custom `SpatialTriggerAction` components to the same GameObject.

## Custom Trigger Action

```csharp
using GameKit.Spatial;
using UnityEngine;

public sealed class DoorTriggerAction : SpatialTriggerAction
{
    [SerializeField] private Animator doorAnimator;

    public override void OnEnter(GameObject target)
    {
        doorAnimator.SetBool("Open", true);
    }

    public override void OnExit(GameObject target)
    {
        doorAnimator.SetBool("Open", false);
    }
}
```

`SpatialTrigger` calls every attached action on enter and exit.

## Trigger Once

Enable `Trigger Once` when enter behavior should happen only once for the lifetime of the component. Use this for pickups, one-time reveals, or tutorial triggers.

## Runtime Lookup and Toggle

```csharp
using GameKit.Spatial;

SpatialTrigger.Toggle("courtyard_gate_trigger", false);

if (SpatialTrigger.TryGet("courtyard_gate_trigger", out SpatialTrigger trigger))
    trigger.SetActiveState(true);
```

`Unique Id` values must be unique. Duplicate ids log an error and the duplicate object is not registered.

## SpatialSfx Setup

1. Add `SpatialSfx` to a GameObject.
2. An `AudioSource` and `SphereCollider` are required.
3. Assign `Clip`.
4. Configure `Target` layer/tag and `Trigger Radius`.
5. Configure 3D audio fields: `Min Distance`, `Max Distance`, and `Rolloff Mode`.

## SpatialSfx Behavior

- On matching enter, playback starts when `Play On Enter` is enabled.
- Multiple matching objects may be inside the trigger at once.
- Playback stops on exit only when `Stop On Exit` is enabled and the last matching object exits.
- If `Is Looping` is enabled, playback repeats after the clip length plus `Delay Before Looping`.
- Disabling the component clears tracked occupants and stops playback.

## Save and Restore

Both `SpatialTrigger` and `SpatialSfx` implement `ISaveable`.

```csharp
object state = trigger.CaptureState();
trigger.RestoreState(state);
```

Their save state records:

- `id`
- `isActive`

Your project owns persistence. Store the returned state under `SaveKey`.

## Proper Use

- Give saveable spatial objects stable, unique ids.
- Use layer masks for broad filtering and tags only when needed.
- Keep custom `SpatialTriggerAction` classes small and focused.
- Avoid relying on `OnExit` for one-time triggers that disable or destroy themselves immediately.
- Put the collider radius and audio max distance in sensible relationship. The trigger controls when audio starts; audio distance controls how it attenuates.
