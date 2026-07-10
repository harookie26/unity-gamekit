using System.Collections.Generic;

namespace GameKit.Events
{
    public sealed class EventPayload
    {
        private readonly Dictionary<string, object> values = new();

        public void Set<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            values[key] = value;
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (values.TryGetValue(key, out object raw) && raw is T typed)
            {
                value = typed;
                return true;
            }

            value = default;
            return false;
        }

        public T Get<T>(string key, T fallback = default)
        {
            return TryGet(key, out T value) ? value : fallback;
        }
    }
}
