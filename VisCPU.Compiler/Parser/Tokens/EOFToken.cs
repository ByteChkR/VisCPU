using VisCPU.Utility;

namespace VisCPU.Compiler.Parser.Tokens
{
    public class EOFToken : AToken
    {

        public EOFToken(string originalText, int start, int length) : base(originalText, start, length)
        {
        }

        public override string GetValue()
        {
            return "<EOF>";
        }

    }
}