using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Dialogue
{
    [Serializable]
    public sealed class DialogueResponse
    {
        [Tooltip("Text displayed on the response button.")]
        public string text = "";

        [TextArea(2, 4)]
        [Tooltip("Optional longer description available to custom choice presenters.")]
        public string description = "";

        [Tooltip("Outcomes executed in order after this response is selected.")]
        public List<DialogueOutcome> outcomes = new();
    }

    [CreateAssetMenu(fileName = "BranchingDialogue", menuName = "GameKit/Dialogue/Branching Dialogue")]
    public sealed class BranchingDialogue : DialogueAsset
    {
        [Tooltip("Optional dialogue played before the responses are shown.")]
        public DialogueAsset prompt;

        [Tooltip("Player responses and the ordered outcomes caused by each response.")]
        public List<DialogueResponse> responses = new();
    }
}
