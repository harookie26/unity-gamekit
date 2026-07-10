namespace GameKit.Dialogue
{
    public sealed class DialoguePlaybackHandle
    {
        public bool IsComplete { get; private set; }
        public bool WasInterrupted { get; private set; }

        internal void Complete()
        {
            IsComplete = true;
        }

        internal void Interrupt()
        {
            WasInterrupted = true;
            IsComplete = true;
        }
    }
}
