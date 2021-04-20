using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Parser.Tokens
{

    public class EofToken : AToken
    {

        #region Public

        public EofToken( string originalText, int start, int length ) : base( originalText, start, length )
        {
        }

        public override string GetValue()
        {
            return "<EOF>";
        }

        #endregion

    }

}
