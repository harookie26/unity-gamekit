namespace GameKit.Core
{
    public static class CutsceneGate
    {
        private static int activeCount;

        public static bool IsActive => activeCount > 0;
        public static int ActiveCount => activeCount;

        public static bool Begin()
        {
            activeCount++;
            return activeCount == 1;
        }

        public static bool End()
        {
            activeCount = System.Math.Max(0, activeCount - 1);
            return activeCount == 0;
        }

        public static void Reset()
        {
            activeCount = 0;
        }
    }
}
