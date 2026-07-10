using System.Collections;
using TMPro;
using UnityEngine;

namespace GameKit.Dialogue
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class TMPTextAnimator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private void Reset()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Awake()
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
        }

        public void PlayTypewriter(string message, float charDelay = 0.05f)
        {
            StopAllCoroutines();
            StartCoroutine(Typewriter(message, charDelay));
        }

        public void PlayFadeInWhole(string message, float duration = 0.5f)
        {
            StopAllCoroutines();
            StartCoroutine(FadeWhole(message, duration));
        }

        private IEnumerator Typewriter(string message, float charDelay)
        {
            text.text = message ?? "";
            text.ForceMeshUpdate();
            text.maxVisibleCharacters = 0;

            int total = text.textInfo.characterCount;
            for (int i = 1; i <= total; i++)
            {
                text.maxVisibleCharacters = i;
                yield return new WaitForSecondsRealtime(charDelay);
            }
        }

        private IEnumerator FadeWhole(string message, float duration)
        {
            text.text = message ?? "";
            text.ForceMeshUpdate();

            CanvasGroup group = text.GetComponent<CanvasGroup>() ?? text.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;

            if (duration <= 0f)
            {
                group.alpha = 1f;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }

            group.alpha = 1f;
        }
    }
}
