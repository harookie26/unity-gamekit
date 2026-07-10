using System.Collections;
using System.Collections.Generic;
using GameKit.Interaction;
using UnityEngine;

namespace GameKit.Spatial
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(SphereCollider))]
    public sealed class SpatialSfx : MonoBehaviour, ISaveable
    {
        private static readonly Dictionary<string, SpatialSfx> Registry = new();

        [Header("Identification")]
        [SerializeField] private string uniqueId;

        [Header("Detection")]
        [SerializeField] private DetectionTarget target = new();
        [SerializeField] private float triggerRadius = 5f;

        [Header("Audio")]
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool isLooping = true;
        [SerializeField] private float delayBeforeLooping;
        [SerializeField] private bool playOnEnter = true;
        [SerializeField] private bool stopOnExit = true;

        [Header("3D Audio")]
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float maxDistance = 15f;
        [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

        private AudioSource audioSource;
        private Coroutine loopRoutine;
        private readonly HashSet<GameObject> inside = new();
        private bool isActive = true;

        public string SaveKey => uniqueId;
        public bool IsActive => isActive;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = rolloffMode;
            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;

            SphereCollider trigger = GetComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = triggerRadius;
            trigger.center = Vector3.zero;

            Register();
        }

        private void OnDestroy()
        {
            Unregister();
        }

        private void OnDisable()
        {
            inside.Clear();
            StopPlayback();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive || !target.Matches(other))
                return;

            inside.Add(other.gameObject);

            if (playOnEnter)
                StartPlayback();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!target.Matches(other))
                return;

            inside.Remove(other.gameObject);

            if (stopOnExit && inside.Count == 0)
                StopPlayback();
        }

        public void SetActiveState(bool state)
        {
            isActive = state;
            if (!isActive)
            {
                inside.Clear();
                StopPlayback();
            }
        }

        public object CaptureState()
        {
            return new SpatialSfxState
            {
                id = uniqueId,
                isActive = isActive
            };
        }

        public void RestoreState(object state)
        {
            if (state is SpatialSfxState sfxState)
                SetActiveState(sfxState.isActive);
        }

        public static bool TryGet(string id, out SpatialSfx sfx)
        {
            return Registry.TryGetValue(id, out sfx);
        }

        public static void Toggle(string id, bool state)
        {
            if (Registry.TryGetValue(id, out SpatialSfx sfx))
                sfx.SetActiveState(state);
        }

        private void StartPlayback()
        {
            if (loopRoutine != null || audioSource.clip == null)
                return;

            loopRoutine = StartCoroutine(LoopRoutine());
        }

        private IEnumerator LoopRoutine()
        {
            do
            {
                if (!isActive)
                {
                    StopPlayback();
                    yield break;
                }

                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);

                if (!isLooping)
                    break;

                if (delayBeforeLooping > 0f)
                    yield return new WaitForSeconds(delayBeforeLooping);
            }
            while (isLooping);

            loopRoutine = null;
        }

        private void StopPlayback()
        {
            if (loopRoutine != null)
            {
                StopCoroutine(loopRoutine);
                loopRoutine = null;
            }

            if (audioSource != null)
                audioSource.Stop();
        }

        private void Register()
        {
            if (string.IsNullOrWhiteSpace(uniqueId))
                return;

            if (Registry.TryGetValue(uniqueId, out SpatialSfx existing) && existing != this)
            {
                Debug.LogError($"Duplicate SpatialSfx uniqueId '{uniqueId}' found. Static lookup and save state require unique ids.", this);
                return;
            }

            Registry[uniqueId] = this;
        }

        private void Unregister()
        {
            if (!string.IsNullOrWhiteSpace(uniqueId) && Registry.TryGetValue(uniqueId, out SpatialSfx sfx) && sfx == this)
                Registry.Remove(uniqueId);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxDistance);
        }
    }
}
