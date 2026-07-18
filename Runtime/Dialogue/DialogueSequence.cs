using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueSequence", menuName = "GameKit/Dialogue/Dialogue Sequence")]
    public sealed class DialogueSequence : DialogueAsset
    {
        [Tooltip("Dialogue entries played in order. Entries may be voiced or non-voiced.")]
        public List<DialogueEntry> entries = new();
    }
}
