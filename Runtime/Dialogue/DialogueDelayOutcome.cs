using System.Collections;
using UnityEngine;

namespace GameKit.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueDelayOutcome", menuName = "GameKit/Dialogue/Outcomes/Delay")]
    public sealed class DialogueDelayOutcome : DialogueOutcome
    {
        [Min(0f)]
        [SerializeField] private float seconds = 0.5f;
        [SerializeField] private bool useUnscaledTime = true;

        public override IEnumerator Execute(DialogueOutcomeContext context)
        {
            if (seconds <= 0f)
                yield break;

            if (useUnscaledTime)
                yield return new WaitForSecondsRealtime(seconds);
            else
                yield return new WaitForSeconds(seconds);
        }
    }
}
