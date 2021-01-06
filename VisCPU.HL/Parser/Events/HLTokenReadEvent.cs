using System.Collections.Generic;
using System.Text;

using VisCPU.HL.Parser.Tokens;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

/// <summary>
/// Contains Parser Specific Exceptions
/// </summary>
namespace VisCPU.HL.Parser.Events
{

    /// <summary>
    ///     Occurs if the Parser Encounters a Token that is unexpected
    /// </summary>
    public class HLTokenReadEvent : ErrorEvent
    {

        /// <summary>
        ///     The Expected Tokens
        /// </summary>
        private readonly HLTokenType[] expected;

        /// <summary>
        ///     The Sequence that was unexpected
        /// </summary>
        private readonly IEnumerable < IHLToken > sequence;

        /// <summary>
        ///     The Token that led to the Exception
        /// </summary>
        private readonly HLTokenType unmatched;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="tokenSequence">Token Sequence</param>
        /// <param name="expected">Expected Tokens</param>
        /// <param name="unmatched">Unmatched Token</param>
        /// <param name="start">Start index in source</param>
        public HLTokenReadEvent(
            IEnumerable < IHLToken > tokenSequence,
            HLTokenType[] expected,
            HLTokenType unmatched,
            int start ) :
            base(
                 $"Expected '{GetExpectedTokenString( expected )}' but got '{unmatched} at index {start}'",
                 ErrorEventKeys.HL_INVALID_TOKEN,
                 false
                )
        {
            sequence = tokenSequence;
            this.expected = expected;
            this.unmatched = unmatched;
        }

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="tokenSequence">Token Sequence</param>
        /// <param name="expected">Expected Token</param>
        /// <param name="unmatched">Unmatched Token</param>
        /// <param name="start">Start index in source</param>
        public HLTokenReadEvent(
            IEnumerable < IHLToken > tokenSequence,
            HLTokenType expected,
            HLTokenType unmatched,
            int start ) : this(
                               tokenSequence,
                               new[] { expected },
                               unmatched,
                               start
                              )
        {
        }

        public HLTokenReadEvent( HLTokenType expected, HLTokenType got ) : base(
                                                                                $"Expected Token '{expected}' but got '{got}'",
                                                                                ErrorEventKeys.HL_INVALID_TOKEN,
                                                                                false
                                                                               )
        {
        }

        #endregion

        #region Private

        /// <summary>
        ///     Returns the string representation of the expected tokens
        /// </summary>
        /// <param name="expected">Expected Tokens</param>
        /// <returns></returns>
        private static string GetExpectedTokenString( HLTokenType[] expected )
        {
            StringBuilder sb = new StringBuilder();

            for ( int i = 0; i < expected.Length; i++ )
            {
                sb.Append( expected[i] );

                if ( i != expected.Length - 1 )
                {
                    sb.Append( ", " );
                }
            }

            return sb.ToString();
        }

        #endregion

    }

}
