using VisCPU.Utility;

namespace VisCPU.Compiler.Parser.Tokens
{
    public abstract class ValueToken : AToken
    {

        public ValueToken(string originalText, int start, int length) : base(originalText, start, length)
        {
        }

        public abstract uint Value { get; }

        public override string GetValue()
        {
            return OriginalText.Substring(Start, Length);
        }

    }
}