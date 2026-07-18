using UnityEngine;

namespace GameKit.Dialogue
{
    [DisallowMultipleComponent]
    public sealed class DialoguePlayer : MonoBehaviour
    {
        [SerializeField] private DialogueAsset dialogue;
        [SerializeField] private bool playOnStart;

        public DialoguePlaybackHandle CurrentPlayback { get; private set; }

        private void Start()
        {
            if (playOnStart)
                Play();
        }

        public void Play()
        {
            if (DialogueManager.Instance == null)
            {
                Debug.LogError("DialoguePlayer requires an active DialogueManager in the scene.", this);
                return;
            }

            CurrentPlayback = DialogueManager.Instance.Play(dialogue);
        }

        public void Stop()
        {
            DialogueManager.Instance?.StopCurrentDialogue();
        }
    }
}
