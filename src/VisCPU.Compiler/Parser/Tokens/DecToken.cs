using VisCPU.Compiler.Parser.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Compiler.Parser.Tokens
{

    public class DecToken : ValueToken
    {
        public override uint Value
        {
            get
            {
                string val = GetValue();

                if ( uint.TryParse( val, out uint hexVal ) )
                {
                    return hexVal;
                }

                EventManager < ErrorEvent >.SendEvent( new InvalidDecValueEvent( val, Start ) );

                return 0;
            }
        }

        #region Public

        public DecToken( string originalText, int start, int length ) : base( originalText, start, length )
        {
        }

        public override string ToString()
        {
            return base.ToString() + $"({Value})";
        }

        #endregion
    }

}
