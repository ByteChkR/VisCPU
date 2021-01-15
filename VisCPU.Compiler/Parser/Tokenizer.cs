using System;
using System.Collections.Generic;

using VisCPU.Compiler.Parser.Tokens;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Parser
{

    public class Tokenizer : VisBase
    {

        private readonly string m_OriginalText;
        private int m_Position;

        protected override LoggerSystems SubSystem => LoggerSystems.Parser;

        private char Current => Peek( 0 );

        #region Public

        public static List < AToken[] > Tokenize( string text )
        {
            Tokenizer reader = new Tokenizer( text );
            List < AToken > tokens = new List < AToken > { new NewLineToken( text, 0, -1 ) };

            while ( reader.Current != '\0' )
            {
                AToken current = reader.Advance();

                if ( current is EOFToken )
                {
                    break;
                }

                tokens.Add( current );
            }

            List < AToken[] > asmInstructions = new List < AToken[] >();
            List < AToken > asmInstruction = new List < AToken >();

            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( tokens[i] is NewLineToken )
                {
                    if ( asmInstruction.Count != 0 )
                    {
                        asmInstructions.Add( asmInstruction.ToArray() );
                    }

                    asmInstruction.Clear();

                    continue;
                }

                asmInstruction.Add( tokens[i] );
            }

            if ( asmInstruction.Count != 0 )
            {
                asmInstructions.Add( asmInstruction.ToArray() );
            }

            asmInstruction.Clear();

            return asmInstructions;
        }

        #endregion

        #region Private

        private Tokenizer( string text )
        {
            m_OriginalText = text;
        }

        private AToken Advance()
        {
            if ( Current == '\0' )
            {
                return new EOFToken( m_OriginalText, m_Position, 0 );
            }

            ReadUntil( BeginningOfWord );

            if ( Current == ';' )
            {
                ReadUntil( EndOfSingleLineComment );

                return Advance();
            }

            int start = m_Position;

            if ( Current == '"' )
            {
                char sepItem = Current;
                m_Position++;
                int len = ReadUntil( x => x == sepItem );
                m_Position++;

                return new StringToken( m_OriginalText, start, len + 2 );
            }

            if ( Current == ':' )
            {
                int len = ReadUntil( EndOfWord );

                return new WordToken( m_OriginalText, start, len ).Resolve();
            }

            if ( Current == '\n' )
            {
                int size = 1;
                m_Position += size;

                return new NewLineToken( m_OriginalText, start, size );
            }

            if ( Current == '\r' )
            {
                int size = 1;

                if ( Peek( 1 ) == '\n' )
                {
                    size++;
                }

                m_Position += size;

                return new NewLineToken( m_OriginalText, start, size );
            }

            if ( Current == '\'' )
            {
                char sepItem = Current;
                start++;
                m_Position++;
                int len = ReadUntil( x => x == sepItem );
                m_Position++;

                return new CharToken( m_OriginalText, start, len );
            }

            if ( char.IsDigit( Current ) )
            {
                char f = Current;
                char s = Peek( 1 );
                int len = ReadUntil( EndOfWord );

                if ( f == '0' && s == 'x' )
                {
                    return new HexToken( m_OriginalText, start, len );
                }

                return new DecToken( m_OriginalText, start, len );
            }
            else
            {
                int len = ReadUntil( EndOfWord );

                return new WordToken( m_OriginalText, start, len ).Resolve();
            }
        }

        private bool BeginningOfConstantInstruction( char input )
        {
            return input == ':';
        }

        private bool BeginningOfWord( char input )
        {
            return char.IsLetterOrDigit( input ) ||
                   input == '_' ||
                   BeginningOfConstantInstruction( input ) ||
                   input == '\n' ||
                   input == '\r' ||
                   input == ':' ||
                   input == ';' ||
                   input == '.' ||
                   input == '"' ||
                   input == '\'';
        }

        private bool EndOfSingleLineComment( char input )
        {
            return input == '\n' || input == '\r';
        }

        private bool EndOfWord( char input )
        {
            return IsWhiteSpace( input ) || input == ';' || input == '\n' || input == '\r';
        }

        private bool IsWhiteSpace( char input )
        {
            return input == '\t' || input == ' ';
        }

        private char Peek( int offset )
        {
            return m_Position + offset < m_OriginalText.Length ? m_OriginalText[m_Position + offset] : '\0';
        }

        private int ReadUntil( Func < char, bool > func )
        {
            int start = m_Position;

            while ( Current != '\0' && !func( Current ) )
            {
                m_Position++;
            }

            return m_Position - start;
        }

        #endregion

    }

}
