using UnityEngine;

namespace GameKit.Spatial
{
    [System.Serializable]
    public sealed class DetectionTarget
    {
        [SerializeField] private string requiredTag = "";
        [SerializeField] private LayerMask layerMask = ~0;

        public string RequiredTag
        {
            get => requiredTag;
            set => requiredTag = value;
        }

        public LayerMask LayerMask
        {
            get => layerMask;
            set => layerMask = value;
        }

        public bool Matches(Collider collider)
        {
            if (collider == null)
                return false;

            if ((layerMask.value & (1 << collider.gameObject.layer)) == 0)
                return false;

            return string.IsNullOrWhiteSpace(requiredTag) || collider.CompareTag(requiredTag);
        }
    }
}
