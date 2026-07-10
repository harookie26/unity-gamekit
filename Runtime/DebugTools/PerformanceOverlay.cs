using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameKit.DebugTools
{
    public sealed class PerformanceOverlay : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private TextMeshProUGUI sceneNameText;
        [SerializeField] private TextMeshProUGUI resolutionText;

        [Header("Behavior")]
        [SerializeField] private float updateInterval = 0.25f;
        [SerializeField] private bool showMs = true;
        [SerializeField] private bool visibleOnAwake = true;

        private float smoothedDeltaTime;
        private float nextUpdateTime;
        private bool isVisible;

        private void Awake()
        {
            isVisible = visibleOnAwake;
            if (fpsText == null)
                fpsText = GetComponentInChildren<TextMeshProUGUI>(true);

            ApplyVisibility();
        }

        private void Start()
        {
            RefreshStaticText();
        }

        private void Update()
        {
            if (!isVisible || fpsText == null)
                return;

            float delta = Time.unscaledDeltaTime;
            smoothedDeltaTime += (delta - smoothedDeltaTime) * 0.1f;

            if (Time.unscaledTime < nextUpdateTime)
                return;

            nextUpdateTime = Time.unscaledTime + Mathf.Max(0.05f, updateInterval);
            float fps = smoothedDeltaTime > 0f ? 1f / smoothedDeltaTime : 0f;

            fpsText.text = showMs
                ? $"{fps:0.} FPS ({smoothedDeltaTime * 1000f:0.0} ms)"
                : $"{fps:0.} FPS";
        }

        public void Toggle()
        {
            SetVisible(!isVisible);
        }

        public void SetVisible(bool visible)
        {
            isVisible = visible;
            ApplyVisibility();
        }

        public void RefreshStaticText()
        {
            if (sceneNameText != null)
                sceneNameText.text = $"Scene: {SceneManager.GetActiveScene().name}";

            if (resolutionText != null)
                resolutionText.text = $"{Screen.width}x{Screen.height}";
        }

        private void ApplyVisibility()
        {
            if (fpsText != null)
                fpsText.gameObject.SetActive(isVisible);

            if (sceneNameText != null)
                sceneNameText.gameObject.SetActive(isVisible);

            if (resolutionText != null)
                resolutionText.gameObject.SetActive(isVisible);
        }
    }
}
