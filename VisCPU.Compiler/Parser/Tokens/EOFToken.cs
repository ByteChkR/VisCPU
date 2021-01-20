using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Parser.Tokens
{

    public class EOFToken : AToken
    {
        #region Public

        public EOFToken( string originalText, int start, int length ) : base( originalText, start, length )
        {
        }

        public override string GetValue()
        {
            return "<EOF>";
        }

        #endregion
    }

}
