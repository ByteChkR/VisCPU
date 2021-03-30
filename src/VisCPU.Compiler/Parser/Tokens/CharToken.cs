using System.Text.RegularExpressions;
using VisCPU.Compiler.Parser.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Compiler.Parser.Tokens
{

    public class CharToken : ValueToken
    {
        public override uint Value
        {
            get
            {
                string val = GetValue();

                if ( char.TryParse(
                    Regex.Unescape( val ),
                    out char chr
                ) )
                {
                    return chr;
                }

                EventManager < ErrorEvent >.SendEvent( new InvalidCharValueEvent( val, Start ) );

                return 0;
            }
        }

        #region Public

        public CharToken( string originalText, int start, int length ) : base( originalText, start, length )
        {
        }

        public override string ToString()
        {
            return base.ToString() + $"({Value})";
        }

        #endregion
    }

}
