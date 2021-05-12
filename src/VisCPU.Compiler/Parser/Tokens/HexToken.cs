using System.Globalization;

using VisCPU.Compiler.Parser.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Compiler.Parser.Tokens
{

    public class HexToken : ValueToken
    {

        public override uint Value
        {
            get
            {
                string val = GetValue().Remove( 0, 2 );

                if ( uint.TryParse( val, NumberStyles.HexNumber, null, out uint hexVal ) )
                {
                    return hexVal;
                }

                EventManager < ErrorEvent >.SendEvent( new InvalidDecValueEvent( val, Start ) );

                return 0;
            }
        }

        #region Public

        public HexToken( string originalText, int start, int length ) : base( originalText, start, length )
        {
        }

        public override string ToString()
        {
            return base.ToString() + $"({Value})";
        }

        #endregion

    }

}
