namespace VisCPU.Compiler.Parser.Tokens
{
    public class ValToken : ValueToken
    {

        public ValToken(string originalText, int start, int length, uint value) : base(originalText, start, length)
        {
            Value = value;
        }

        public override uint Value { get; }

        public override string ToString()
        {
            return base.ToString() + $"({Value})";
        }

    }
}