using System.Collections;
using UnityEngine;

namespace GameKit.Dialogue
{
    [CreateAssetMenu(fileName = "DialoguePlaybackOutcome", menuName = "GameKit/Dialogue/Outcomes/Play Dialogue")]
    public sealed class DialoguePlaybackOutcome : DialogueOutcome
    {
        [SerializeField] private DialogueAsset dialogue;

        public override IEnumerator Execute(DialogueOutcomeContext context)
        {
            if (context == null || dialogue == null)
                yield break;

            yield return context.Play(dialogue);
        }
    }
}
