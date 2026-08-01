using System;
using System.Collections.Generic;
using GameKit.Events;
using UnityEngine;
using UnityEngine.Events;

namespace GameKit.Dialogue
{
    [Serializable]
    public sealed class DialoguePayloadEvent : UnityEvent<EventPayload>
    {
    }

    [Serializable]
    public sealed class DialogueEventBinding
    {
        public string eventName = "";
        public UnityEvent onRaised = new();
        public DialoguePayloadEvent onRaisedWithPayload = new();
    }

    [DisallowMultipleComponent]
    public sealed class DialogueEventRelay : MonoBehaviour
    {
        [SerializeField] private List<DialogueEventBinding> bindings = new();

        private sealed class Registration
        {
            public DialogueEventBinding Binding;
            public Action<EventPayload> Listener;
        }

        private readonly List<Registration> listeners = new();

        private void OnEnable()
        {
            foreach (DialogueEventBinding binding in bindings)
            {
                if (binding == null || string.IsNullOrWhiteSpace(binding.eventName))
                    continue;

                DialogueEventBinding captured = binding;
                Action<EventPayload> listener = payload =>
                {
                    captured.onRaised?.Invoke();
                    captured.onRaisedWithPayload?.Invoke(payload);
                };

                listeners.Add(new Registration { Binding = binding, Listener = listener });
                GlobalEventBus.Instance.Add(binding.eventName, listener);
            }
        }

        private void OnDisable()
        {
            foreach (Registration registration in listeners)
            {
                if (registration.Binding != null && !string.IsNullOrWhiteSpace(registration.Binding.eventName))
                    GlobalEventBus.Instance.Remove(registration.Binding.eventName, registration.Listener);
            }

            listeners.Clear();
        }
    }
}
