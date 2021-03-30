using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Parser.Tokens
{

    public abstract class ValueToken : AToken
    {
        public abstract uint Value { get; }

        #region Public

        public override string GetValue()
        {
            return OriginalText.Substring( Start, Length );
        }

        #endregion

        #region Protected

        protected ValueToken( string originalText, int start, int length ) : base( originalText, start, length )
        {
        }

        #endregion
    }

}
