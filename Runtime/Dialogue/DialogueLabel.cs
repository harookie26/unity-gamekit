using System.Collections;
using TMPro;
using UnityEngine;

namespace GameKit.Dialogue
{
    public sealed class DialogueLabel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private CanvasGroup canvasGroup;

        private Coroutine routine;

        private void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            bodyText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        }

        public void Show(string characterName, string text, float fadeIn, float hold, float fadeOut)
        {
            if (routine != null)
                StopCoroutine(routine);

            if (characterNameText != null)
            {
                characterNameText.text = characterName ?? "";
                characterNameText.gameObject.SetActive(!string.IsNullOrWhiteSpace(characterName));
            }

            if (bodyText != null)
                bodyText.text = text ?? "";

            gameObject.SetActive(true);
            routine = StartCoroutine(ShowRoutine(fadeIn, hold, fadeOut));
        }

        public void HideImmediately()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            if (canvasGroup != null)
                canvasGroup.alpha = 0f;

            gameObject.SetActive(false);
        }

        private IEnumerator ShowRoutine(float fadeIn, float hold, float fadeOut)
        {
            yield return Fade(0f, 1f, fadeIn);
            if (hold > 0f)
                yield return new WaitForSecondsRealtime(hold);
            yield return Fade(1f, 0f, fadeOut);
            routine = null;
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            if (canvasGroup == null)
                yield break;

            if (duration <= 0f)
            {
                canvasGroup.alpha = to;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            canvasGroup.alpha = to;
        }
    }
}
