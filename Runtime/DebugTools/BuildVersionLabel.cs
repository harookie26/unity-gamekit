using TMPro;
using UnityEngine;

namespace GameKit.DebugTools
{
    public sealed class BuildVersionLabel : MonoBehaviour
    {
        [SerializeField] private string buildVersion = "Development Build";
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private bool visibleOnAwake = true;

        private bool isVisible;

        private void Reset()
        {
            label = GetComponent<TextMeshProUGUI>() ?? GetComponentInChildren<TextMeshProUGUI>(true);
        }

        private void Awake()
        {
            if (label == null)
                label = GetComponent<TextMeshProUGUI>() ?? GetComponentInChildren<TextMeshProUGUI>(true);

            isVisible = visibleOnAwake;
            Apply();
        }

        private void Start()
        {
            if (label != null)
                label.text = buildVersion;
        }

        public void SetBuildVersion(string value)
        {
            buildVersion = value;
            if (label != null)
                label.text = buildVersion;
        }

        public void Toggle()
        {
            SetVisible(!isVisible);
        }

        public void SetVisible(bool visible)
        {
            isVisible = visible;
            Apply();
        }

        private void Apply()
        {
            if (label != null)
                label.gameObject.SetActive(isVisible);
        }
    }
}
