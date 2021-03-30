namespace VisCPU.Compiler.Parser.Tokens
{

    public class StringToken : WordToken
    {
        private WordToken Content => new WordToken( OriginalText, Start + 1, Length - 2 );

        #region Public

        public StringToken( string originalText, int start, int length ) : base( originalText, start, length )
        {
        }

        public string GetContent()
        {
            return Content.GetValue();
        }

        #endregion
    }

}
