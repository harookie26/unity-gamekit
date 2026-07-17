# Dialogue Starter

This sample provides a ready-to-use dialogue canvas, label, non-voiced sequence, and example player.

## Try the sample

1. Drag `Prefabs/Dialogue Canvas` into an open scene.
2. Drag `Prefabs/Example Dialogue Player` into the same scene.
3. Enter Play mode. The example sequence plays automatically.

The Dialogue Canvas prefab already contains a configured `DialogueManager`, `AudioSource`, dialogue container, and label prefab reference. Keep only one active `DialogueManager` in a scene.

Create your own sequence from `Assets > Create > GameKit > Dialogue > Dialogue Sequence`. Add `DialogueEntry` or `VoicedDialogueEntry` assets to its Entries list, then assign that one sequence asset to your gameplay script.
