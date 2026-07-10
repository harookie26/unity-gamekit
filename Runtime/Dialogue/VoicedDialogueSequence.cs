using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Dialogue
{
    [System.Serializable]
    public sealed class TimedDialogueLine
    {
        public string characterName = "";

        [TextArea(2, 6)]
        public string text = "";

        [Tooltip("Seconds from voice clip start when this line begins.")]
        public float startTime = 0f;

        [Tooltip("Seconds from voice clip start when this line ends. If 0, next line start or clip length is used.")]
        public float endTime = 0f;

        public float fadeIn = 0.25f;
        public float fadeOut = 0.25f;
    }

    [CreateAssetMenu(fileName = "VoicedDialogueSequence", menuName = "GameKit/Dialogue/Voiced Dialogue Sequence")]
    public sealed class VoicedDialogueSequence : ScriptableObject
    {
        public AudioClip voiceClip;
        public List<TimedDialogueLine> lines = new();
    }
}
