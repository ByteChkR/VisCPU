using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Parser.Tokens
{

    public class NewLineToken : AToken
    {

        #region Public

        public NewLineToken( string originalText, int start, int length ) : base( originalText, start, length )
        {
        }

        public override string GetValue()
        {
            return "<NEWL>";
        }

        #endregion

    }

}
