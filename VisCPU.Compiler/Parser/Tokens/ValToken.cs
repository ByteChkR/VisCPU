namespace VisCPU.Compiler.Parser.Tokens
{
    public class ValToken : ValueToken
    {
        public override uint Value { get; }

        #region Public

        public ValToken(string originalText, int start, int length, uint value) : base(originalText, start, length)
        {
            Value = value;
        }

        public override string ToString()
        {
            return base.ToString() + $"({Value})";
        }

        #endregion
    }
}