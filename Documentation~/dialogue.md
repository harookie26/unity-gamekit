# Dialogue

Namespace: `GameKit.Dialogue`

The Dialogue system displays TMP subtitle/dialogue labels, queues dialogue lines, supports individual voiced lines, and can sync subtitle lines to a single voiced sequence clip.

Requires TextMeshPro.

## Types

- `DialogueManager`: scene manager for queued playback and voiced sequences.
- `DialogueLabel`: visual label prefab component.
- `DialogueEntry`: ScriptableObject for one dialogue line.
- `DialogueSequence`: ScriptableObject containing an ordered list of dialogue entries.
- `VoicedDialogueEntry`: dialogue line with an `AudioClip`.
- `VoicedDialogueSequence`: ScriptableObject containing one voice clip and timed subtitle lines.
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
- An `Example Dialogue Player` prefab that plays the sequence on startup.

Drag `Dialogue Canvas` and `Example Dialogue Player` into a scene, then enter Play mode.

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
- Do not create multiple active managers in additive scenes unless you intentionally control which one survives.
