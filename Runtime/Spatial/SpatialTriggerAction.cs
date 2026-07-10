using UnityEngine;

namespace GameKit.Spatial
{
    public abstract class SpatialTriggerAction : MonoBehaviour
    {
        public virtual void OnEnter(GameObject target)
        {
        }

        public virtual void OnExit(GameObject target)
        {
        }
    }
}
