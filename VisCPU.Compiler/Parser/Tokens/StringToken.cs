namespace VisCPU.Compiler.Parser.Tokens
{
    public class StringToken : WordToken
    {

        public StringToken(string originalText, int start, int length) : base(originalText, start, length)
        {
        }

        public WordToken Content => new WordToken(OriginalText, Start + 1, Length - 2);

    }
}