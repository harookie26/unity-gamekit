# Editor

Namespace: `GameKit.Editor`

The Editor system currently provides a custom inspector for MonoBehaviours marked with `FoldableInspectorAttribute`.

## Foldable Component Inspector

Add `[FoldableInspector]` to a MonoBehaviour class. The custom editor groups serialized fields into foldouts using Unity `[Header]` attributes.

```csharp
using GameKit.Core;
using UnityEngine;

[FoldableInspector(displayName: "Enemy Patrol")]
public sealed class EnemyPatrol : MonoBehaviour
{
    [Header("Route")]
    [SerializeField] private Transform[] points;
    [SerializeField] private float speed = 3f;

    [Header("Detection")]
    [SerializeField] private float viewDistance = 12f;
    [SerializeField] private LayerMask targetMask;
}
```

## Behavior

- The script field is shown read-only.
- Each `[Header]` starts a new foldout group.
- Fields before the first `[Header]` appear under `Settings`.
- Foldout state is remembered by component type and group header while the editor session is active.
- Tooltip attributes are forwarded to the rendered property field.

## Attribute Options

```csharp
[FoldableInspector]
[FoldableInspector(displayName: "Door Controller")]
[FoldableInspector(hideFieldHeaders: true, displayName: "Door Controller")]
```

`DisplayName` changes the help-box title at the top of the inspector.

## Proper Use

- Use headers to define meaningful groups such as `Setup`, `Runtime`, `Audio`, and `Debug`.
- Keep fields serialized normally; the inspector reads Unity serialized properties.
- Do not use this for custom editors that require bespoke controls, previews, scene handles, or validation buttons.
- If a component needs specialized editor behavior, write a dedicated `CustomEditor` for that component.
