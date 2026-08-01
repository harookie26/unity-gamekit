using System.Collections;
using UnityEngine;

namespace GameKit.Dialogue
{
    public abstract class DialogueOutcome : ScriptableObject
    {
        public abstract IEnumerator Execute(DialogueOutcomeContext context);
    }

    public sealed class DialogueOutcomeContext
    {
        public DialogueManager Manager { get; }
        public BranchingDialogue Conversation { get; }
        public DialogueResponse Response { get; }
        public int ResponseIndex { get; }

        internal DialogueOutcomeContext(
            DialogueManager manager,
            BranchingDialogue conversation,
            DialogueResponse response,
            int responseIndex)
        {
            Manager = manager;
            Conversation = conversation;
            Response = response;
            ResponseIndex = responseIndex;
        }

        public IEnumerator Play(DialogueAsset dialogue)
        {
            return Manager != null ? Manager.PlayAsOutcome(dialogue) : Empty();
        }

        private static IEnumerator Empty()
        {
            yield break;
        }
    }
}
