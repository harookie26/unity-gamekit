namespace GameKit.Dialogue
{
    public sealed class DialoguePlaybackHandle
    {
        public bool IsComplete { get; private set; }
        public bool WasInterrupted { get; private set; }
        public int SelectedResponseIndex { get; private set; } = -1;

        internal void SelectResponse(int index)
        {
            SelectedResponseIndex = index;
        }

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
