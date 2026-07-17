using GameKit.Dialogue;
using UnityEngine;

namespace GameKit.Samples.Dialogue
{
    public sealed class DialogueSequencePlayer : MonoBehaviour
    {
        [SerializeField] private DialogueSequence sequence;
        [SerializeField] private bool playOnStart = true;

        private void Start()
        {
            if (playOnStart)
                Play();
        }

        public void Play()
        {
            if (DialogueManager.Instance == null)
            {
                Debug.LogError("Add the Dialogue Canvas prefab to the scene before playing a dialogue sequence.", this);
                return;
            }

            DialogueManager.Instance.DisplaySequence(sequence);
        }
    }
}
