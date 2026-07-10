namespace GameKit.Events
{
    public static class GlobalEventBus
    {
        public static EventBus Instance { get; } = new EventBus();

        public static void Clear()
        {
            Instance.Clear();
        }
    }
}
