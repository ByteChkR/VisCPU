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
        private readonly string input;

        /// <summary>
        ///     XL Settings
        /// </summary>
        private readonly HLParserSettings settings;

        /// <summary>
        ///     The Current Reader Index.
        /// </summary>
        private int currentIndex;

        /// <summary>
        ///     The Current Token
        /// </summary>
        private IHLToken currentToken;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="settings">XL Settings</param>
        /// <param name="input">Source</param>
        public HLParserBaseReader(HLParserSettings settings, string input)
        {
            this.input = input;
            this.settings = settings;
        }

        public List<IHLToken> ReadToEnd()
        {
            if (currentToken == null)
            {
                Advance();
            }

            List<IHLToken> ret = new List<IHLToken>();
            while (currentToken.Type != HLTokenType.EOF)
            {
                ret.Add(currentToken);
                Advance();
            }

            return ret;
        }

        /// <summary>
        ///     Advances the reader by one position inside the source tokens
        /// </summary>
        /// <returns></returns>
        public IHLToken Advance()
        {
            if (currentIndex < input.Length)
            {
                if (IsNewLine(input[currentIndex]))
                {
                    currentToken = ReadNewLine();
                }
                else if (IsSpace(input[currentIndex]))
                {
                    currentIndex++;
                    return Advance();
                }
                else if (IsSymbol(input[currentIndex]))
                {
                    currentToken = ReadSymbol();
                }
                else if (input[currentIndex] == '0' &&
                         input.Length - 2 > currentIndex &&
                         input[currentIndex + 1] == 'x' &&
                         IsHexNumber(input[currentIndex + 2]))
                {
                    currentToken = ReadHexNumber();
                }
                else if (IsNumber(input[currentIndex]))
                {
                    currentToken = ReadNumber();
                }
                else if (IsLetter(input[currentIndex]))
                {
                    currentToken = ReadWord();
                }
                else
                {
                    currentToken = new HLTextToken(HLTokenType.Unknown, input[currentIndex].ToString(), currentIndex);
                    currentIndex++;
                }

                return currentToken;
            }

            currentToken = new EOFToken();

            return currentToken;
        }


        /// <summary>
        ///     Returns true if the Character is a Reserved Symbol
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns>True if Reserved Symbol</returns>
        private bool IsSymbol(char c)
        {
            return settings.ReservedSymbols.ContainsKey(c);
        }


        /// <summary>
        ///     Returns true if the Character is a Number
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns>True if number</returns>
        private static bool IsNumber(char c)
        {
            return char.IsDigit(c);
        }

        private static bool IsHexNumber(char c)
        {
            return char.IsDigit(c) || char.ToUpper(c) >= 'A' && char.ToUpper(c) <= 'F';
        }

        /// <summary>
        ///     Returns true if the character is a letter or underscore
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns>True if Letter or Underscore</returns>
        private static bool IsLetter(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        /// <summary>
        ///     Reads a sequence of spaces until the first "non-space" character is encountered.
        /// </summary>
        /// <returns></returns>
        private IHLToken ReadSpace()
        {
            int len = 0;
            int start = currentIndex;
            do
            {
                len++;
                currentIndex++;
            } while (currentIndex < input.Length && IsSpace(input[currentIndex]));

            return new HLTextToken(HLTokenType.OpSpace, new StringBuilder().Append(' ', len).ToString(), start);
        }

        /// <summary>
        ///     Reads all new Lines until a "non-newline" character is encountered.
        /// </summary>
        /// <returns></returns>
        private IHLToken ReadNewLine()
        {
            int len = 0;
            int start = currentIndex;
            do
            {
                len++;
                currentIndex++;
            } while (currentIndex < input.Length && IsNewLine(input[currentIndex]));

            return new HLTextToken(HLTokenType.OpNewLine, new StringBuilder().Append('\n', len).ToString(), start);
        }

        /// <summary>
        ///     Reads a Word from the Source
        /// </summary>
        /// <returns></returns>
        private IHLToken ReadWord()
        {
            int start = currentIndex;
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(input[currentIndex]);
                currentIndex++;
            } while (currentIndex < input.Length && (IsNumber(input[currentIndex]) || IsLetter(input[currentIndex])));

            return new HLTextToken(HLTokenType.OpWord, sb.ToString(), start);
        }

        /// <summary>
        ///     reads a number from the source
        /// </summary>
        /// <returns></returns>
        private IHLToken ReadNumber()
        {
            int start = currentIndex;
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(input[currentIndex]);
                currentIndex++;
            } while (currentIndex < input.Length && IsNumber(input[currentIndex]));

            return new HLTextToken(HLTokenType.OpNumber, sb.ToString(), start);
        }

        private IHLToken ReadHexNumber()
        {
            int start = currentIndex;
            StringBuilder sb = new StringBuilder("0x");
            currentIndex += 2;
            do
            {
                sb.Append(input[currentIndex]);
                currentIndex++;
            } while (currentIndex < input.Length && IsHexNumber(input[currentIndex]));

            return new HLTextToken(HLTokenType.OpNumber, sb.ToString(), start);
        }

        /// <summary>
        ///     Reads a Symbol from the Source
        /// </summary>
        /// <returns></returns>
        private IHLToken ReadSymbol()
        {
            char val = input[currentIndex];
            int start = currentIndex;
            currentIndex++;
            return new HLTextToken(settings.ReservedSymbols[val], val.ToString(), start);
        }

        /// <summary>
        ///     Returns true if the character is a new line '\n'
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns></returns>
        private static bool IsNewLine(char c)
        {
            return c == '\n';
        }

        /// <summary>
        ///     Returns true if the character is a new line ' ' || '\t' || '\r'
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns></returns>
        private static bool IsSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r';
        }

    }
}