using System.Linq;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Parser.Tokens
{

    public class WordToken : AToken
    {
        #region Public

        public WordToken( string originalText, int start, int length ) : base( originalText, start, length )
        {
        }

        public override string GetValue()
        {
            return OriginalText.Substring( Start, Length );
        }

        public AToken Resolve()
        {
            if ( GetValue().All( char.IsDigit ) )
            {
                return new DecToken( OriginalText, Start, Length );
            }

            return this;
        }

        #endregion
    }

}
