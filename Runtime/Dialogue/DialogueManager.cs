using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Dialogue
{
    public sealed class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("Setup")]
        [SerializeField] private RectTransform dialogueContainer;
        [SerializeField] private DialogueLabel labelPrefab;
        [SerializeField] private int initialPool = 2;
        [SerializeField] private AudioSource voiceAudioSource;
        [SerializeField] private DialogueChoicePresenter choicePresenter;

        private struct DialogueRequest
        {
            public string CharacterName;
            public string Text;
            public float FadeIn;
            public float DisplayDuration;
            public float FadeOut;
            public AudioClip VoiceLine;
            public DialoguePlaybackHandle Handle;
        }

        private readonly Queue<DialogueLabel> pool = new();
        private readonly Queue<DialogueRequest> queue = new();
        private DialogueLabel currentLabel;
        private DialogueRequest currentRequest;
        private Coroutine finishCoroutine;
        private Coroutine sequenceCoroutine;
        private Coroutine branchingCoroutine;
        private DialoguePlaybackHandle activeSequenceHandle;
        private DialoguePlaybackHandle activeBranchingHandle;
        private bool isPlaying;
        private bool isReady;

        public event Action<BranchingDialogue, DialogueResponse, int> ResponseSelected;

        public DialoguePlaybackHandle Play(DialogueAsset dialogue)
        {
            switch (dialogue)
            {
                case DialogueEntry entry:
                    return DisplaySequence(new[] { entry });
                case DialogueSequence sequence:
                    return DisplaySequence(sequence);
                case VoicedDialogueSequence voicedSequence:
                    return DisplaySequence(voicedSequence);
                case BranchingDialogue branchingDialogue:
                    return DisplayBranching(branchingDialogue);
                case null:
                    DialoguePlaybackHandle emptyHandle = new();
                    emptyHandle.Complete();
                    return emptyHandle;
                default:
                    DialoguePlaybackHandle unsupportedHandle = new();
                    unsupportedHandle.Interrupt();
                    Debug.LogError($"Unsupported dialogue asset type: {dialogue.GetType().Name}.", dialogue);
                    return unsupportedHandle;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (voiceAudioSource == null)
                voiceAudioSource = GetComponent<AudioSource>();

            if (labelPrefab == null)
            {
                Debug.LogError("DialogueManager requires a DialogueLabel prefab before it can display dialogue.", this);
                return;
            }

            if (labelPrefab.gameObject.scene.IsValid())
                labelPrefab.HideImmediately();

            for (int i = 0; i < initialPool; i++)
                pool.Enqueue(CreateNew());

            isReady = true;
        }

        private DialogueLabel CreateNew()
        {
            DialogueLabel label = Instantiate(labelPrefab, dialogueContainer);
            label.gameObject.SetActive(false);
            return label;
        }

        private DialogueLabel GetLabel()
        {
            DialogueLabel label = pool.Count > 0 ? pool.Dequeue() : CreateNew();
            label.gameObject.SetActive(true);
            return label;
        }

        private void ReturnLabel(DialogueLabel label)
        {
            if (label == null)
                return;

            label.HideImmediately();
            pool.Enqueue(label);
        }

        public void Display(DialogueEntry entry)
        {
            if (entry == null)
                return;

            AudioClip clip = entry is VoicedDialogueEntry voiced ? voiced.voiceLine : null;
            Display(entry.characterName, entry.text, entry.fadeIn, entry.displayDuration, entry.fadeOut, clip);
        }

        public void Display(string characterName, string text, float fadeIn = 0.25f, float hold = 3f, float fadeOut = 0.25f, AudioClip voiceLine = null)
        {
            if (!EnsureReady())
                return;

            queue.Enqueue(new DialogueRequest
            {
                CharacterName = characterName,
                Text = text,
                FadeIn = fadeIn,
                DisplayDuration = hold,
                FadeOut = fadeOut,
                VoiceLine = voiceLine
            });

            if (!isPlaying)
                ProcessQueue();
        }

        public DialoguePlaybackHandle DisplaySequence(IEnumerable<DialogueEntry> entries)
        {
            return DisplaySequence(entries, true);
        }

        private DialoguePlaybackHandle DisplaySequence(IEnumerable<DialogueEntry> entries, bool stopCurrent)
        {
            DialoguePlaybackHandle handle = new();
            if (!EnsureReady())
            {
                handle.Interrupt();
                return handle;
            }

            if (entries == null)
            {
                handle.Complete();
                return handle;
            }

            if (stopCurrent)
                StopCurrentDialogue();
            else
                StopPresentation();

            foreach (DialogueEntry entry in entries)
            {
                if (entry == null)
                    continue;

                queue.Enqueue(new DialogueRequest
                {
                    CharacterName = entry.characterName,
                    Text = entry.text,
                    FadeIn = entry.fadeIn,
                    DisplayDuration = entry.displayDuration,
                    FadeOut = entry.fadeOut,
                    VoiceLine = entry is VoicedDialogueEntry voiced ? voiced.voiceLine : null,
                    Handle = handle
                });
            }

            if (queue.Count == 0)
                handle.Complete();
            else
                ProcessQueue();

            return handle;
        }

        public DialoguePlaybackHandle DisplaySequence(DialogueSequence sequence)
        {
            return DisplaySequence(sequence != null ? sequence.entries : null);
        }

        public DialoguePlaybackHandle DisplaySequence(VoicedDialogueSequence sequence)
        {
            return DisplaySequence(sequence, true);
        }

        private DialoguePlaybackHandle DisplaySequence(VoicedDialogueSequence sequence, bool stopCurrent)
        {
            DialoguePlaybackHandle handle = new();
            if (!EnsureReady())
            {
                handle.Interrupt();
                return handle;
            }

            if (sequence == null)
            {
                handle.Complete();
                return handle;
            }

            if (stopCurrent)
                StopCurrentDialogue();
            else
                StopPresentation();

            activeSequenceHandle = handle;
            sequenceCoroutine = StartCoroutine(PlayVoicedSequence(sequence, handle));
            return handle;
        }

        public DialoguePlaybackHandle DisplayBranching(BranchingDialogue conversation)
        {
            DialoguePlaybackHandle handle = new();
            if (!EnsureReady())
            {
                handle.Interrupt();
                return handle;
            }

            if (conversation == null)
            {
                handle.Complete();
                return handle;
            }

            if (choicePresenter == null || !choicePresenter.CanPresent)
            {
                Debug.LogError("Branching dialogue requires a configured DialogueChoicePresenter.", this);
                handle.Interrupt();
                return handle;
            }

            StopCurrentDialogue();
            activeBranchingHandle = handle;
            branchingCoroutine = StartCoroutine(RunBranching(conversation, handle));
            return handle;
        }

        public void StopCurrentDialogue()
        {
            if (branchingCoroutine != null)
            {
                StopCoroutine(branchingCoroutine);
                branchingCoroutine = null;
            }

            choicePresenter?.Hide();
            StopPresentation();

            activeBranchingHandle?.Interrupt();
            activeBranchingHandle = null;
        }

        private void StopPresentation()
        {
            if (sequenceCoroutine != null)
            {
                StopCoroutine(sequenceCoroutine);
                sequenceCoroutine = null;
            }

            if (finishCoroutine != null)
            {
                StopCoroutine(finishCoroutine);
                finishCoroutine = null;
            }

            if (currentLabel != null)
            {
                ReturnLabel(currentLabel);
                currentLabel = null;
            }

            if (voiceAudioSource != null)
            {
                voiceAudioSource.Stop();
                voiceAudioSource.clip = null;
            }

            currentRequest.Handle?.Interrupt();
            activeSequenceHandle?.Interrupt();
            activeSequenceHandle = null;

            foreach (DialogueRequest request in queue)
                request.Handle?.Interrupt();

            currentRequest = default;
            queue.Clear();
            isPlaying = false;
        }

        internal IEnumerator PlayAsOutcome(DialogueAsset dialogue)
        {
            if (dialogue == null)
                yield break;

            if (dialogue is BranchingDialogue branchingDialogue)
            {
                DialoguePlaybackHandle nestedHandle = new();
                yield return RunBranching(branchingDialogue, nestedHandle);
                yield break;
            }

            DialoguePlaybackHandle handle = PlayWithoutStopping(dialogue);
            while (!handle.IsComplete)
                yield return null;
        }

        private DialoguePlaybackHandle PlayWithoutStopping(DialogueAsset dialogue)
        {
            switch (dialogue)
            {
                case DialogueEntry entry:
                    return DisplaySequence(new[] { entry }, false);
                case DialogueSequence sequence:
                    return DisplaySequence(sequence != null ? sequence.entries : null, false);
                case VoicedDialogueSequence voicedSequence:
                    return DisplaySequence(voicedSequence, false);
                default:
                    DialoguePlaybackHandle handle = new();
                    handle.Interrupt();
                    Debug.LogError($"Unsupported dialogue outcome asset type: {dialogue.GetType().Name}.", dialogue);
                    return handle;
            }
        }

        private IEnumerator RunBranching(BranchingDialogue conversation, DialoguePlaybackHandle handle)
        {
            if (conversation.prompt != null)
                yield return PlayAsOutcome(conversation.prompt);

            if (handle.WasInterrupted)
                yield break;

            if (conversation.responses == null || conversation.responses.Count == 0)
            {
                handle.Complete();
                FinishBranching(handle);
                yield break;
            }

            int selectedIndex = -1;
            choicePresenter.Show(conversation.responses, index => selectedIndex = index);
            if (choicePresenter.PresentedChoiceCount == 0)
            {
                Debug.LogWarning($"Branching dialogue '{conversation.name}' has no presentable responses.", conversation);
                handle.Complete();
                FinishBranching(handle);
                yield break;
            }

            while (selectedIndex < 0)
                yield return null;

            choicePresenter.Hide();
            if (selectedIndex >= conversation.responses.Count)
            {
                handle.Interrupt();
                FinishBranching(handle);
                yield break;
            }

            DialogueResponse response = conversation.responses[selectedIndex];
            handle.SelectResponse(selectedIndex);
            ResponseSelected?.Invoke(conversation, response, selectedIndex);

            if (handle.WasInterrupted)
                yield break;

            if (response?.outcomes != null)
            {
                DialogueOutcomeContext context = new(this, conversation, response, selectedIndex);
                foreach (DialogueOutcome outcome in response.outcomes)
                {
                    if (outcome != null)
                        yield return outcome.Execute(context);
                }
            }

            handle.Complete();
            FinishBranching(handle);
        }

        private void FinishBranching(DialoguePlaybackHandle handle)
        {
            if (activeBranchingHandle != handle)
                return;

            branchingCoroutine = null;
            activeBranchingHandle = null;
        }

        private void ProcessQueue()
        {
            if (queue.Count == 0 || isPlaying || sequenceCoroutine != null)
                return;

            currentRequest = queue.Dequeue();
            isPlaying = true;
            currentLabel = GetLabel();

            float holdDuration = currentRequest.DisplayDuration;
            if (currentRequest.VoiceLine != null)
            {
                holdDuration = Mathf.Max(0f, currentRequest.VoiceLine.length - currentRequest.FadeIn);
                if (voiceAudioSource != null)
                {
                    voiceAudioSource.Stop();
                    voiceAudioSource.clip = currentRequest.VoiceLine;
                    voiceAudioSource.Play();
                }
            }

            currentLabel.Show(currentRequest.CharacterName, currentRequest.Text, currentRequest.FadeIn, holdDuration, currentRequest.FadeOut);
            float totalDuration = currentRequest.FadeIn + holdDuration + currentRequest.FadeOut + 0.05f;
            finishCoroutine = StartCoroutine(FinishCurrentDialogue(totalDuration));
        }

        private IEnumerator FinishCurrentDialogue(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);

            ReturnLabel(currentLabel);
            currentLabel = null;
            finishCoroutine = null;
            isPlaying = false;

            if (voiceAudioSource != null)
            {
                voiceAudioSource.Stop();
                voiceAudioSource.clip = null;
            }

            DialoguePlaybackHandle handle = currentRequest.Handle;
            currentRequest = default;

            if (!QueueContainsHandle(handle))
                handle?.Complete();

            ProcessQueue();
        }

        private IEnumerator PlayVoicedSequence(VoicedDialogueSequence sequence, DialoguePlaybackHandle handle)
        {
            double startTime = AudioSettings.dspTime;

            if (sequence.voiceClip != null && voiceAudioSource != null)
            {
                voiceAudioSource.Stop();
                voiceAudioSource.clip = sequence.voiceClip;
                startTime = AudioSettings.dspTime + 0.05d;
                voiceAudioSource.PlayScheduled(startTime);
            }

            float sequenceLength = Mathf.Max(sequence.voiceClip != null ? sequence.voiceClip.length : 0f, GetFallbackLength(sequence));
            int count = sequence.lines != null ? sequence.lines.Count : 0;

            for (int i = 0; i < count; i++)
            {
                TimedDialogueLine line = sequence.lines[i];
                if (line == null || string.IsNullOrEmpty(line.text))
                    continue;

                float start = Mathf.Max(0f, line.startTime);
                float end = ResolveLineEnd(sequence, i, sequenceLength);
                if (end <= start)
                    continue;

                yield return WaitForSequenceTime(startTime, start);

                currentLabel = GetLabel();
                float total = end - start;
                float fadeIn = Mathf.Max(0f, line.fadeIn);
                float fadeOut = Mathf.Max(0f, line.fadeOut);
                float hold = Mathf.Max(0f, total - fadeIn - fadeOut);
                currentLabel.Show(line.characterName, line.text, fadeIn, hold, fadeOut);

                yield return WaitForSequenceTime(startTime, end);

                ReturnLabel(currentLabel);
                currentLabel = null;
            }

            yield return WaitForSequenceTime(startTime, sequenceLength);

            if (voiceAudioSource != null)
            {
                voiceAudioSource.Stop();
                voiceAudioSource.clip = null;
            }

            sequenceCoroutine = null;
            activeSequenceHandle = null;
            handle.Complete();
            ProcessQueue();
        }

        private static IEnumerator WaitForSequenceTime(double startTime, float targetTime)
        {
            double target = startTime + Mathf.Max(0f, targetTime);
            while (AudioSettings.dspTime < target)
                yield return null;
        }

        private static float ResolveLineEnd(VoicedDialogueSequence sequence, int index, float sequenceLength)
        {
            TimedDialogueLine line = sequence.lines[index];
            if (line.endTime > line.startTime)
                return Mathf.Min(line.endTime, sequenceLength);

            for (int next = index + 1; next < sequence.lines.Count; next++)
            {
                TimedDialogueLine nextLine = sequence.lines[next];
                if (nextLine != null && nextLine.startTime > line.startTime)
                    return Mathf.Min(nextLine.startTime, sequenceLength);
            }

            return sequenceLength;
        }

        private static float GetFallbackLength(VoicedDialogueSequence sequence)
        {
            float length = 0f;
            if (sequence.lines == null)
                return length;

            foreach (TimedDialogueLine line in sequence.lines)
            {
                if (line == null)
                    continue;

                length = Mathf.Max(length, line.endTime, line.startTime + line.fadeIn + line.fadeOut);
            }

            return length;
        }

        private bool QueueContainsHandle(DialoguePlaybackHandle handle)
        {
            if (handle == null)
                return false;

            foreach (DialogueRequest request in queue)
            {
                if (request.Handle == handle)
                    return true;
            }

            return false;
        }

        private bool EnsureReady()
        {
            if (isReady)
                return true;

            Debug.LogError("DialogueManager is not ready. Assign a DialogueLabel prefab before displaying dialogue.", this);
            return false;
        }
    }
}
