using UnityEngine;

namespace GameKit.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueEntry", menuName = "GameKit/Dialogue/Dialogue Entry")]
    public class DialogueEntry : DialogueAsset
    {
        [Tooltip("Character name shown with this dialogue line.")]
        public string characterName = "";

        [TextArea(2, 6)]
        public string text = "";

        [Tooltip("Seconds to hold the line at full alpha, excluding fade durations.")]
        public float displayDuration = 3f;

        public float fadeIn = 0.25f;
        public float fadeOut = 0.25f;
    }
}
