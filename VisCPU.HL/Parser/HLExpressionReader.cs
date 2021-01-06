using System.Collections.Generic;
using System.Linq;
using System.Text;

using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     Contains Shared Logic between different Parser Stages.
    /// </summary>
    /// <summary>
    ///     XLang Expression Reader Implementation
    /// </summary>
    public class HLExpressionReader
    {

        /// <summary>
        ///     Input Token Stream
        /// </summary>
        public readonly List < IHLToken > Tokens;

        /// <summary>
        ///     The Current Position inside the token Stream
        /// </summary>
        private int currentIdx;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        public HLExpressionReader( List < IHLToken > tokens )
        {
            Tokens = tokens.ToList();
        }

        /// <summary>
        ///     Advances the Reader by one position and returns the read token
        /// </summary>
        /// <returns></returns>
        public IHLToken GetNext()
        {
            HLParsingTools.ReadAnyOrNone( Tokens, currentIdx, out IHLToken result );
            currentIdx++;

            return result;
        }

        /// <summary>
        ///     Peeks into the next (or specified) position relative to the current position
        /// </summary>
        /// <param name="advance">Relative offset to the current position</param>
        /// <returns>Token at the specified Position</returns>
        public IHLToken PeekNext( int advance )
        {
            HLParsingTools.ReadAnyOrNone( Tokens, currentIdx + advance - 1, out IHLToken result );

            return result;
        }

        /// <summary>
        ///     Peeks into the next (or specified) position relative to the current position
        /// </summary>
        /// <returns>Token at the specified Position</returns>
        public IHLToken PeekNext()
        {
            return PeekNext( 1 );
        }

        public IHLToken PeekPrev()
        {
            return PeekNext( -1 );
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach ( IHLToken hlToken in Tokens )
            {
                sb.Append( $"{hlToken} " );
            }

            return sb.ToString();
        }

        #endregion

    }

}
