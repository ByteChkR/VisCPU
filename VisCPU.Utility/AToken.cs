namespace VisCPU.Utility
{
    public abstract class AToken
    {

        public readonly int Length;
        public readonly string OriginalText;
        public readonly int Start;

        protected AToken(string originalText, int start, int length)
        {
            OriginalText = originalText;
            Start = start;
            Length = length;
        }

        public abstract string GetValue();

        public override string ToString()
        {
            return $"[{base.ToString()}]" + GetValue();
        }

    }
}