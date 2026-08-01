# Dialogue

Namespace: `GameKit.Dialogue`

The Dialogue system displays TMP subtitle/dialogue labels, queues dialogue lines, supports individual voiced lines, syncs subtitles to voice clips, and runs branching conversations with extensible gameplay outcomes.

Requires TextMeshPro.

## Types

- `DialogueAsset`: shared base type accepted by unified playback fields and APIs.
- `DialogueManager`: scene manager for queued playback and voiced sequences.
- `DialoguePlayer`: reusable component that plays any `DialogueAsset` from one Inspector field.
- `DialogueLabel`: visual label prefab component.
- `DialogueEntry`: ScriptableObject for one dialogue line.
- `DialogueSequence`: ScriptableObject containing an ordered list of dialogue entries.
- `VoicedDialogueEntry`: dialogue line with an `AudioClip`.
- `VoicedDialogueSequence`: ScriptableObject containing one voice clip and timed subtitle lines.
- `BranchingDialogue`: prompt, player responses, and ordered outcome lists.
- `DialogueChoicePresenter`: response-button UI used by branching dialogue.
- `DialogueOutcome`: extensible ScriptableObject base for effects caused by a response.
- `DialogueEventRelay`: connects named dialogue outcome events to scene UnityEvents.
- `DialoguePlaybackHandle`: completion/interruption status for sequence playback.
- `TMPTextAnimator`: typewriter or fade-in animation helper for a TMP text component.

## Dialogue Label Prefab

Create a prefab for `DialogueLabel`.

1. Create a UI object under a Canvas.
2. Add `CanvasGroup`.
3. Add `DialogueLabel`.
4. Assign:
   - `Character Name Text`: optional `TextMeshProUGUI`.
   - `Body Text`: required `TextMeshProUGUI`.
   - `Canvas Group`: the label's `CanvasGroup`.
5. Save it as a prefab.

The label fades in, holds, then fades out using unscaled time.

## DialogueManager Setup

1. Add `DialogueManager` to a scene object.
2. Assign `Dialogue Container` to the parent `RectTransform` where labels should spawn.
3. Assign `Label Prefab`.
4. Set `Initial Pool`.
5. Assign an `AudioSource` if using voice playback, or place an `AudioSource` on the same object.

Only one `DialogueManager` should exist at runtime. The first instance becomes `DialogueManager.Instance`; duplicates destroy themselves.

## Unified Playback

For the simplest setup, add `DialoguePlayer` to a GameObject and assign any supported asset to its `Dialogue` field:

- `DialogueEntry`
- `VoicedDialogueEntry`
- `DialogueSequence`
- `VoicedDialogueSequence`
- `BranchingDialogue`

Enable `Play On Start`, call the component's public `Play()` method from a UnityEvent, or use the unified manager API:

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

The serialized field does not need to change when switching between dialogue types. `Play` returns a `DialoguePlaybackHandle` when called directly on the manager.

## Branching Dialogue and Outcomes

Add a `DialogueChoicePresenter` to a UI object, assign a response container and a Button prefab containing a TMP label, then assign the presenter to `DialogueManager.Choice Presenter`.

Create a branching asset from `Assets > Create > GameKit > Dialogue > Branching Dialogue`:

1. Assign any dialogue asset as the optional prompt.
2. Add the responses shown to the player.
3. Add any number of outcome assets to each response. Outcomes run in list order.

Built-in outcomes are created under `Assets > Create > GameKit > Dialogue > Outcomes`:

- `Play Dialogue` plays any dialogue asset and waits for it, including another branching conversation.
- `Publish Events` publishes multiple named events with string, integer, float, and boolean payload values.
- `Delay` pauses the outcome chain using scaled or unscaled time.

The result remains compatible with the unified playback API:

```csharp
DialoguePlaybackHandle handle = DialogueManager.Instance.Play(branchingDialogue);
while (!handle.IsComplete)
    yield return null;

if (!handle.WasInterrupted)
    Debug.Log($"Selected response {handle.SelectedResponseIndex}");
```

Every published event automatically includes `responseIndex` and `responseText`. Add `DialogueEventRelay` to a scene object to bind event names to parameterless or payload-aware UnityEvents without writing a listener component. Code can also subscribe through `GlobalEventBus`.

For project-specific behavior, derive a ScriptableObject from `DialogueOutcome`. This example unlocks a quest system and can be mixed with every built-in outcome:

```csharp
using System.Collections;
using GameKit.Dialogue;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Dialogue Outcomes/Unlock Quest")]
public sealed class UnlockQuestOutcome : DialogueOutcome
{
    [SerializeField] private string questId;

    public override IEnumerator Execute(DialogueOutcomeContext context)
    {
        QuestManager.Unlock(questId);
        yield break;
    }
}
```

`DialogueManager.ResponseSelected` is also available when code needs immediate notification of the chosen conversation, response, and index.

## Display One Line

```csharp
using GameKit.Dialogue;
using UnityEngine;

public sealed class DialogueTrigger : MonoBehaviour
{
    public void PlayLine()
    {
        DialogueManager.Instance.Display(
            characterName: "Guide",
            text: "Stay close.",
            fadeIn: 0.2f,
            hold: 2.5f,
            fadeOut: 0.2f);
    }
}
```

## Display DialogueEntry

Create an asset from `Assets > Create > GameKit > Dialogue > Dialogue Entry`, fill in the fields, then play it:

```csharp
using GameKit.Dialogue;
using UnityEngine;

public sealed class DialogueAssetPlayer : MonoBehaviour
{
    [SerializeField] private DialogueEntry entry;

    public void Play()
    {
        DialogueManager.Instance.Display(entry);
    }
}
```

If the asset is a `VoicedDialogueEntry`, its `voiceLine` controls the display duration.

## Display a Sequence of Entries

Create an asset from `Assets > Create > GameKit > Dialogue > Dialogue Sequence`, add entries in playback order, then reference the sequence as one asset:

```csharp
using GameKit.Dialogue;
using UnityEngine;

public sealed class ConversationPlayer : MonoBehaviour
{
    [SerializeField] private DialogueSequence sequence;

    public DialoguePlaybackHandle Play()
    {
        return DialogueManager.Instance.DisplaySequence(sequence);
    }
}
```

A sequence may mix `DialogueEntry` and `VoicedDialogueEntry` assets. Each voiced entry uses its own clip for that line.

For sequences assembled dynamically at runtime, pass an enumerable directly:

```csharp
using System.Collections.Generic;
using GameKit.Dialogue;
using UnityEngine;

public sealed class ConversationPlayer : MonoBehaviour
{
    [SerializeField] private List<DialogueEntry> lines;

    public DialoguePlaybackHandle Play()
    {
        return DialogueManager.Instance.DisplaySequence(lines);
    }
}
```

`DisplaySequence(IEnumerable<DialogueEntry>)` stops current dialogue, clears the queue, queues the provided entries, and returns a handle.

## Dialogue Starter Sample

Import `Dialogue Starter` from the package's Samples tab in Package Manager. It includes:

- A configured `Dialogue Canvas` prefab with `DialogueManager` and an `AudioSource`.
- A reusable `Dialogue Label` prefab with TMP name and body text.
- An example `DialogueSequence` and its dialogue entries.
- An `Example Dialogue Player` prefab with one field that accepts every dialogue asset type.
- A ready-to-play `Dialogue Demo` scene containing the complete prefab setup.
- A `Branching Dialogue Demo` scene with choice UI, two response paths, multi-event gameplay outcomes, and visible UnityEvent reactions.

Open `Scenes/Dialogue Demo`, then enter Play mode. The example sequence plays automatically.

Open `Scenes/Branching Dialogue Demo` to try the choice-and-outcome workflow. See the sample README for an asset-by-asset walkthrough and event listener examples.

## Timed Voiced Sequence

Create an asset from `Assets > Create > GameKit > Dialogue > Voiced Dialogue Sequence`.

Set:

- `Voice Clip`: the full spoken clip.
- `Lines`: timed subtitle lines.
- `Start Time`: seconds from voice clip start.
- `End Time`: optional explicit end. If unset, the next line start or clip length is used.

```csharp
using GameKit.Dialogue;
using UnityEngine;

public sealed class CinematicDialogue : MonoBehaviour
{
    [SerializeField] private VoicedDialogueSequence sequence;

    public void Play()
    {
        DialoguePlaybackHandle handle = DialogueManager.Instance.DisplaySequence(sequence);
        StartCoroutine(WaitForDialogue(handle));
    }

    private System.Collections.IEnumerator WaitForDialogue(DialoguePlaybackHandle handle)
    {
        while (!handle.IsComplete)
            yield return null;

        if (!handle.WasInterrupted)
        {
            // Continue cinematic.
        }
    }
}
```

## Stop Playback

```csharp
DialogueManager.Instance.StopCurrentDialogue();
```

This stops active coroutines, returns the current label to the pool, stops voice audio, interrupts active handles, and clears queued requests.

## TMPTextAnimator

Attach `TMPTextAnimator` to a GameObject with `TextMeshProUGUI`.

```csharp
animator.PlayTypewriter("Incoming transmission...", 0.04f);
animator.PlayFadeInWhole("Objective complete.", 0.5f);
```

## Proper Use

- Always assign a `DialogueLabel` prefab. The manager logs and refuses playback when missing.
- Use a dedicated `AudioSource` for dialogue voice so SFX/music systems do not fight it.
- Use unscaled-time behavior intentionally. Dialogue still displays while `Time.timeScale` is zero.
- Keep timed sequence line times sorted by `startTime`.
- Use `DialoguePlaybackHandle` for flow control instead of guessing durations.
- Keep outcome assets focused and combine them in response lists; one response may publish several events and then continue into more dialogue.
- Do not create multiple active managers in additive scenes unless you intentionally control which one survives.
