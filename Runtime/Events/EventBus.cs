using System;
using System.Collections.Generic;

namespace GameKit.Events
{
    public sealed class EventBus
    {
        private readonly Dictionary<string, List<Action>> listeners = new();
        private readonly Dictionary<string, List<Action<EventPayload>>> payloadListeners = new();

        public void Add(string eventName, Action listener)
        {
            if (string.IsNullOrWhiteSpace(eventName) || listener == null)
                return;

            if (!listeners.TryGetValue(eventName, out List<Action> list))
            {
                list = new List<Action>();
                listeners.Add(eventName, list);
            }

            if (!list.Contains(listener))
                list.Add(listener);
        }

        public void Add(string eventName, Action<EventPayload> listener)
        {
            if (string.IsNullOrWhiteSpace(eventName) || listener == null)
                return;

            if (!payloadListeners.TryGetValue(eventName, out List<Action<EventPayload>> list))
            {
                list = new List<Action<EventPayload>>();
                payloadListeners.Add(eventName, list);
            }

            if (!list.Contains(listener))
                list.Add(listener);
        }

        public void Remove(string eventName, Action listener)
        {
            if (string.IsNullOrWhiteSpace(eventName) || listener == null)
                return;

            if (listeners.TryGetValue(eventName, out List<Action> list))
                list.Remove(listener);
        }

        public void Remove(string eventName, Action<EventPayload> listener)
        {
            if (string.IsNullOrWhiteSpace(eventName) || listener == null)
                return;

            if (payloadListeners.TryGetValue(eventName, out List<Action<EventPayload>> list))
                list.Remove(listener);
        }

        public void Publish(string eventName)
        {
            if (!listeners.TryGetValue(eventName, out List<Action> list))
                return;

            Action[] snapshot = list.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
                snapshot[i]?.Invoke();
        }

        public void Publish(string eventName, EventPayload payload)
        {
            if (!payloadListeners.TryGetValue(eventName, out List<Action<EventPayload>> list))
                return;

            Action<EventPayload>[] snapshot = list.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
                snapshot[i]?.Invoke(payload);
        }

        public void Clear()
        {
            listeners.Clear();
            payloadListeners.Clear();
        }
    }
}
