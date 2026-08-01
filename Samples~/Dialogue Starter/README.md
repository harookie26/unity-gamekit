# Dialogue Starter

This sample contains ready-to-play linear and branching dialogue examples. It demonstrates player responses, ordered outcome chains, follow-up dialogue, multiple gameplay events, typed event payloads, and no-code UnityEvent reactions.

## Try the branching demo

1. Open `Scenes/Branching Dialogue Demo`.
2. Enter Play mode and wait for the gatekeeper's prompt.
3. Select a response with the mouse, keyboard, or gamepad.
4. Watch the status text at the top of the screen. It reports the non-dialogue gameplay events caused by the response.
5. A different follow-up line plays for each choice.

The peaceful response publishes `sample.gate.opened` and `sample.inventory.keyGranted`. The forceful response publishes `sample.guards.alerted` and `sample.gate.damaged`. Each branch then plays its own dialogue outcome.

The original non-branching example remains available in `Scenes/Dialogue Demo`.

## What's included

- `Prefabs/Dialogue Canvas`: configured manager, label pool, choice presenter, Graphic Raycaster, event relay, and outcome status text.
- `Prefabs/Dialogue Choice Button`: reusable TMP response button.
- `Prefabs/Branching Dialogue Player`: plays the gate decision automatically.
- `Dialogue/GateDecision`: branching asset containing the prompt and two responses.
- `Dialogue/PeacefulEvents` and `Dialogue/ForcefulEvents`: multi-event gameplay outcomes with typed payload values.
- `Dialogue/PlayPeacefulReply` and `Dialogue/PlayForcefulReply`: follow-up dialogue outcomes.
- `Scenes/Branching Dialogue Demo`: complete playable setup, including an Input System EventSystem.

## Create your own branching conversation

1. Create prompt and follow-up lines from `Assets > Create > GameKit > Dialogue > Dialogue Entry`.
2. Create event outcomes from `Assets > Create > GameKit > Dialogue > Outcomes > Publish Events`.
3. Add as many event names and typed payload values as the response requires.
4. Create follow-up outcomes from `Assets > Create > GameKit > Dialogue > Outcomes > Play Dialogue`.
5. Create the conversation from `Assets > Create > GameKit > Dialogue > Branching Dialogue`.
6. Assign the prompt, add responses, and place outcome assets in execution order.
7. Assign the branching asset to a `DialoguePlayer` or call `DialogueManager.Instance.Play(asset)`.

The sample executes gameplay events before its follow-up line because each response lists the event outcome first. Reorder the assets when another sequence is required.

## Required scene setup

A branching scene needs:

- One active `DialogueManager` with Label Prefab and Choice Presenter assigned.
- A `DialogueChoicePresenter` with a response container and Button prefab.
- A Canvas with a `GraphicRaycaster`.
- An EventSystem compatible with the project's input backend.

The included `Dialogue Canvas` prefab supplies the first three. The branching demo scene supplies an Input System EventSystem.

## React to gameplay events

For no-code reactions, add `DialogueEventRelay`, enter the exact event name, and connect `On Raised` to scene methods through the Inspector. The sample relay changes the outcome status text.

For code-driven systems, subscribe to the global event bus:

```csharp
using GameKit.Events;
using UnityEngine;

public sealed class GateOutcomeListener : MonoBehaviour
{
    private void OnEnable()
    {
        GlobalEventBus.Instance.Add("sample.gate.opened", OnGateOpened);
    }

    private void OnDisable()
    {
        GlobalEventBus.Instance.Remove("sample.gate.opened", OnGateOpened);
    }

    private void OnGateOpened(EventPayload payload)
    {
        string approach = payload.Get("approach", "unknown");
        int reputationDelta = payload.Get("reputationDelta", 0);
        int responseIndex = payload.Get("responseIndex", -1);

        Debug.Log($"{approach}: reputation {reputationDelta}, response {responseIndex}");
    }
}
```

Every dialogue event payload also contains `responseIndex` and `responseText` automatically.

## Extend outcomes beyond the built-ins

Create a ScriptableObject derived from `DialogueOutcome` for quests, inventory changes, AI state, scene transitions, achievements, saving, or any project-specific behavior. Add the resulting asset to a response beside the built-in outcomes.

```csharp
using System.Collections;
using GameKit.Dialogue;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Dialogue Outcomes/Grant Item")]
public sealed class GrantItemOutcome : DialogueOutcome
{
    [SerializeField] private string itemId;

    public override IEnumerator Execute(DialogueOutcomeContext context)
    {
        Inventory.Grant(itemId);
        yield break;
    }
}
```

Use `yield return context.Play(dialogueAsset)` inside a custom outcome when it must wait for another dialogue before the remaining outcomes execute.

## Play any dialogue from a script

```csharp
using GameKit.Dialogue;
using UnityEngine;

public sealed class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueAsset dialogue;

    public void Play()
    {
        DialogueManager.Instance.Play(dialogue);
    }
}
```

The field accepts entries, linear sequences, voiced sequences, and branching dialogue assets.
