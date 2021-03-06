﻿using System.Collections.Generic;
using System.Text;

using VisCPU.HL.Parser.Tokens;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

/// <summary>
/// Contains Parser Specific Exceptions
/// </summary>
namespace VisCPU.HL.Parser.Events
{

    /// <summary>
    ///     Occurs if the Parser Encounters a Token that is unexpected
    /// </summary>
    internal class HlTokenReadEvent : ErrorEvent
    {

        /// <summary>
        ///     The Expected Tokens
        /// </summary>
        private readonly HlTokenType[] m_Expected;

        /// <summary>
        ///     The Sequence that was unexpected
        /// </summary>
        private readonly IEnumerable < IHlToken > m_Sequence;

        /// <summary>
        ///     The Token that led to the Exception
        /// </summary>
        private readonly HlTokenType m_Unmatched;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="tokenSequence">Token Sequence</param>
        /// <param name="expected">Expected Tokens</param>
        /// <param name="unmatched">Unmatched Token</param>
        /// <param name="start">Start index in source</param>
        public HlTokenReadEvent(
            IEnumerable < IHlToken > tokenSequence,
            HlTokenType[] expected,
            HlTokenType unmatched,
            int start ) :
            base(
                 $"Expected '{GetExpectedTokenString( expected )}' but got '{unmatched} at index {start}'",
                 ErrorEventKeys.s_HlInvalidToken,
                 false
                )
        {
            m_Sequence = tokenSequence;
            m_Expected = expected;
            m_Unmatched = unmatched;
        }

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="tokenSequence">Token Sequence</param>
        /// <param name="expected">Expected Token</param>
        /// <param name="unmatched">Unmatched Token</param>
        /// <param name="start">Start index in source</param>
        public HlTokenReadEvent(
            IEnumerable < IHlToken > tokenSequence,
            HlTokenType expected,
            HlTokenType unmatched,
            int start ) : this(
                               tokenSequence,
                               new[] { expected },
                               unmatched,
                               start
                              )
        {
        }

        public HlTokenReadEvent( HlTokenType expected, HlTokenType got ) : base(
                                                                                $"Expected Token '{expected}' but got '{got}'",
                                                                                ErrorEventKeys.s_HlInvalidToken,
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
        private static string GetExpectedTokenString( HlTokenType[] expected )
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
