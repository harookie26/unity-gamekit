using System.Collections.Generic;
using GameKit.Interaction;
using UnityEngine;

namespace GameKit.Spatial
{
    [RequireComponent(typeof(SphereCollider))]
    public sealed class SpatialTrigger : MonoBehaviour, ISaveable
    {
        private static readonly Dictionary<string, SpatialTrigger> Registry = new();

        [Header("Identification")]
        [SerializeField] private string uniqueId;

        [Header("Detection")]
        [SerializeField] private DetectionTarget target = new();
        [SerializeField] private float triggerRadius = 5f;
        [SerializeField] private bool triggerOnce;

        private readonly HashSet<GameObject> inside = new();
        private SpatialTriggerAction[] actions;
        private bool isActive = true;
        private bool hasTriggered;

        public string SaveKey => uniqueId;
        public bool IsActive => isActive;

        private void Awake()
        {
            SphereCollider trigger = GetComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = triggerRadius;
            trigger.center = Vector3.zero;

            actions = GetComponents<SpatialTriggerAction>();
            Register();
        }

        private void OnDestroy()
        {
            Unregister();
        }

        private void OnDisable()
        {
            CleanupAllTargets();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive || (triggerOnce && hasTriggered) || !target.Matches(other))
                return;

            inside.Add(other.gameObject);
            hasTriggered = true;

            for (int i = 0; i < actions.Length; i++)
                actions[i].OnEnter(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isActive || !target.Matches(other))
                return;

            inside.Remove(other.gameObject);

            for (int i = 0; i < actions.Length; i++)
                actions[i].OnExit(other.gameObject);
        }

        public void SetActiveState(bool state)
        {
            isActive = state;
            if (!isActive)
                CleanupAllTargets();
        }

        public object CaptureState()
        {
            return new SpatialTriggerState
            {
                id = uniqueId,
                isActive = isActive
            };
        }

        public void RestoreState(object state)
        {
            if (state is SpatialTriggerState triggerState)
                SetActiveState(triggerState.isActive);
        }

        public static bool TryGet(string id, out SpatialTrigger trigger)
        {
            return Registry.TryGetValue(id, out trigger);
        }

        public static void Toggle(string id, bool state)
        {
            if (Registry.TryGetValue(id, out SpatialTrigger trigger))
                trigger.SetActiveState(state);
        }

        private void Register()
        {
            if (string.IsNullOrWhiteSpace(uniqueId))
                return;

            if (Registry.TryGetValue(uniqueId, out SpatialTrigger existing) && existing != this)
            {
                Debug.LogError($"Duplicate SpatialTrigger uniqueId '{uniqueId}' found. Static lookup and save state require unique ids.", this);
                return;
            }

            Registry[uniqueId] = this;
        }

        private void Unregister()
        {
            if (!string.IsNullOrWhiteSpace(uniqueId) && Registry.TryGetValue(uniqueId, out SpatialTrigger trigger) && trigger == this)
                Registry.Remove(uniqueId);
        }

        private void CleanupAllTargets()
        {
            foreach (GameObject targetObject in inside)
            {
                if (targetObject == null)
                    continue;

                for (int i = 0; i < actions.Length; i++)
                    actions[i].OnExit(targetObject);
            }

            inside.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
        }
    }
}
