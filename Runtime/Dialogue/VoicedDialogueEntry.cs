using UnityEngine;

namespace GameKit.Dialogue
{
    [CreateAssetMenu(fileName = "VoicedDialogueEntry", menuName = "GameKit/Dialogue/Voiced Dialogue Entry")]
    public sealed class VoicedDialogueEntry : DialogueEntry
    {
        [Tooltip("Voice clip for this subtitle line. Clip length controls line duration.")]
        public AudioClip voiceLine;
    }
}
