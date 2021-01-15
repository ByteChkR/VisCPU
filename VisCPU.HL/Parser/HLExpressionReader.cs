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
        ///     The Current Position inside the token Stream
        /// </summary>
        private int m_CurrentIdx;

        /// <summary>
        ///     Input Token Stream
        /// </summary>
        public List < IHlToken > Tokens { get; }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        public HLExpressionReader( List < IHlToken > tokens )
        {
            Tokens = tokens.ToList();
        }

        /// <summary>
        ///     Advances the Reader by one position and returns the read token
        /// </summary>
        /// <returns></returns>
        public IHlToken GetNext()
        {
            HLParsingTools.ReadAnyOrNone( Tokens, m_CurrentIdx, out IHlToken result );
            m_CurrentIdx++;

            return result;
        }

        /// <summary>
        ///     Peeks into the next (or specified) position relative to the current position
        /// </summary>
        /// <param name="advance">Relative offset to the current position</param>
        /// <returns>Token at the specified Position</returns>
        public IHlToken PeekNext( int advance )
        {
            HLParsingTools.ReadAnyOrNone( Tokens, m_CurrentIdx + advance - 1, out IHlToken result );

            return result;
        }

        /// <summary>
        ///     Peeks into the next (or specified) position relative to the current position
        /// </summary>
        /// <returns>Token at the specified Position</returns>
        public IHlToken PeekNext()
        {
            return PeekNext( 1 );
        }

        public IHlToken PeekPrev()
        {
            return PeekNext( -1 );
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach ( IHlToken hlToken in Tokens )
            {
                sb.Append( $"{hlToken} " );
            }

            return sb.ToString();
        }

        #endregion

    }

}
