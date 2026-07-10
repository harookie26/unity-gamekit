using System.Collections.Generic;

namespace GameKit.Core
{
    public sealed class RuntimeRegistry<T> where T : class
    {
        private readonly List<T> items = new();

        public IReadOnlyList<T> Items => items;

        public void Register(T item)
        {
            if (item == null || items.Contains(item))
                return;

            items.Add(item);
        }

        public void Unregister(T item)
        {
            items.Remove(item);
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}
