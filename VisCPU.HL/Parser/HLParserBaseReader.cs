using System.Collections.Generic;
using System.Text;

using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     Contains the Different Token Reader Implementations for the Different parsing phases.
    /// </summary>
    /// <summary>
    ///     XLang base Reader that is used by the BoardParser
    /// </summary>
    public class HLParserBaseReader
    {

        /// <summary>
        ///     Input Source
        /// </summary>
        private readonly string m_Input;

        /// <summary>
        ///     XL SettingsSystem
        /// </summary>
        private readonly HLParserSettings m_Settings;

        /// <summary>
        ///     The Current Reader Index.
        /// </summary>
        private int m_CurrentIndex;

        /// <summary>
        ///     The Current Token
        /// </summary>
        private IHlToken m_CurrentToken;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="settings">XL SettingsSystem</param>
        /// <param name="input">Source</param>
        public HLParserBaseReader( HLParserSettings settings, string input )
        {
            m_Input = input;
            m_Settings = settings;
        }

        /// <summary>
        ///     Advances the reader by one position inside the source tokens
        /// </summary>
        /// <returns></returns>
        public IHlToken Advance()
        {
            if ( m_CurrentIndex < m_Input.Length )
            {
                if ( IsNewLine( m_Input[m_CurrentIndex] ) )
                {
                    m_CurrentToken = ReadNewLine();
                }
                else if ( IsSpace( m_Input[m_CurrentIndex] ) )
                {
                    m_CurrentIndex++;

                    return Advance();
                }
                else if ( IsSymbol( m_Input[m_CurrentIndex] ) )
                {
                    m_CurrentToken = ReadSymbol();
                }
                else if ( m_Input[m_CurrentIndex] == '0' &&
                          m_Input.Length - 2 > m_CurrentIndex &&
                          m_Input[m_CurrentIndex + 1] == 'x' &&
                          IsHexNumber( m_Input[m_CurrentIndex + 2] ) )
                {
                    m_CurrentToken = ReadHexNumber();
                }
                else if ( IsNumber( m_Input[m_CurrentIndex] ) )
                {
                    m_CurrentToken = ReadNumber();
                }
                else if ( IsLetter( m_Input[m_CurrentIndex] ) )
                {
                    m_CurrentToken = ReadWord();
                }
                else
                {
                    m_CurrentToken = new HLTextToken(
                                                     HLTokenType.Unknown,
                                                     m_Input[m_CurrentIndex].ToString(),
                                                     m_CurrentIndex
                                                    );

                    m_CurrentIndex++;
                }

                return m_CurrentToken;
            }

            m_CurrentToken = new EOFToken();

            return m_CurrentToken;
        }

        public List < IHlToken > ReadToEnd()
        {
            if ( m_CurrentToken == null )
            {
                Advance();
            }

            List < IHlToken > ret = new List < IHlToken >();

            while ( m_CurrentToken.Type != HLTokenType.Eof )
            {
                ret.Add( m_CurrentToken );
                Advance();
            }

            return ret;
        }

        #endregion

        #region Private

        private static bool IsHexNumber( char c )
        {
            return char.IsDigit( c ) || char.ToUpper( c ) >= 'A' && char.ToUpper( c ) <= 'F';
        }

        /// <summary>
        ///     Returns true if the character is a letter or underscore
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns>True if Letter or Underscore</returns>
        private static bool IsLetter( char c )
        {
            return char.IsLetter( c ) || c == '_';
        }

        /// <summary>
        ///     Returns true if the character is a new line '\n'
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns></returns>
        private static bool IsNewLine( char c )
        {
            return c == '\n';
        }

        /// <summary>
        ///     Returns true if the Character is a Number
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns>True if number</returns>
        private static bool IsNumber( char c )
        {
            return char.IsDigit( c );
        }

        /// <summary>
        ///     Returns true if the character is a new line ' ' || '\t' || '\r'
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns></returns>
        private static bool IsSpace( char c )
        {
            return c == ' ' || c == '\t' || c == '\r';
        }

        /// <summary>
        ///     Returns true if the Character is a Reserved Symbol
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns>True if Reserved Symbol</returns>
        private bool IsSymbol( char c )
        {
            return m_Settings.ReservedSymbols.ContainsKey( c );
        }

        private IHlToken ReadHexNumber()
        {
            int start = m_CurrentIndex;
            StringBuilder sb = new StringBuilder( "0x" );
            m_CurrentIndex += 2;

            do
            {
                sb.Append( m_Input[m_CurrentIndex] );
                m_CurrentIndex++;
            }
            while ( m_CurrentIndex < m_Input.Length && IsHexNumber( m_Input[m_CurrentIndex] ) );

            return new HLTextToken( HLTokenType.OpNumber, sb.ToString(), start );
        }

        /// <summary>
        ///     Reads all new Lines until a "non-newline" character is encountered.
        /// </summary>
        /// <returns></returns>
        private IHlToken ReadNewLine()
        {
            int len = 0;
            int start = m_CurrentIndex;

            do
            {
                len++;
                m_CurrentIndex++;
            }
            while ( m_CurrentIndex < m_Input.Length && IsNewLine( m_Input[m_CurrentIndex] ) );

            return new HLTextToken( HLTokenType.OpNewLine, new StringBuilder().Append( '\n', len ).ToString(), start );
        }

        /// <summary>
        ///     reads a number from the source
        /// </summary>
        /// <returns></returns>
        private IHlToken ReadNumber()
        {
            int start = m_CurrentIndex;
            StringBuilder sb = new StringBuilder();

            do
            {
                sb.Append( m_Input[m_CurrentIndex] );
                m_CurrentIndex++;
            }
            while ( m_CurrentIndex < m_Input.Length && IsNumber( m_Input[m_CurrentIndex] ) );

            return new HLTextToken( HLTokenType.OpNumber, sb.ToString(), start );
        }

        /// <summary>
        ///     Reads a sequence of spaces until the first "non-space" character is encountered.
        /// </summary>
        /// <returns></returns>
        private IHlToken ReadSpace()
        {
            int len = 0;
            int start = m_CurrentIndex;

            do
            {
                len++;
                m_CurrentIndex++;
            }
            while ( m_CurrentIndex < m_Input.Length && IsSpace( m_Input[m_CurrentIndex] ) );

            return new HLTextToken( HLTokenType.OpSpace, new StringBuilder().Append( ' ', len ).ToString(), start );
        }

        /// <summary>
        ///     Reads a Symbol from the Source
        /// </summary>
        /// <returns></returns>
        private IHlToken ReadSymbol()
        {
            char val = m_Input[m_CurrentIndex];
            int start = m_CurrentIndex;
            m_CurrentIndex++;

            return new HLTextToken( m_Settings.ReservedSymbols[val], val.ToString(), start );
        }

        /// <summary>
        ///     Reads a Word from the Source
        /// </summary>
        /// <returns></returns>
        private IHlToken ReadWord()
        {
            int start = m_CurrentIndex;
            StringBuilder sb = new StringBuilder();

            do
            {
                sb.Append( m_Input[m_CurrentIndex] );
                m_CurrentIndex++;
            }
            while ( m_CurrentIndex < m_Input.Length &&
                    ( IsNumber( m_Input[m_CurrentIndex] ) || IsLetter( m_Input[m_CurrentIndex] ) ) );

            return new HLTextToken( HLTokenType.OpWord, sb.ToString(), start );
        }

        #endregion

    }

}
