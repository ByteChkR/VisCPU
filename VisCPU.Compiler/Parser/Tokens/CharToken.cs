using System;
using System.Text.RegularExpressions;

using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Parser.Tokens
{
    public class InvalidCharValueEvent : ErrorEvent
    {

        private const string EVENT_KEY = "invalid-char-value";
        public InvalidCharValueEvent( string value, int start ) : base($"Invalid char Value: '{value}' at line {start}", EVENT_KEY, false )
        {
        }

    }
    
    public class CharToken : ValueToken
    {

        public CharToken(string originalText, int start, int length) : base(originalText, start, length)
        {
        }

        public override uint Value
        {
            get
            {
                string val = GetValue();
                if (char.TryParse(
                                  Regex.Unescape(val),
                                  out char chr
                                 ))
                {
                    return chr;
                }

                EventManager < ErrorEvent >.SendEvent( new InvalidCharValueEvent( val, Start ) );

                return 0;
            }
        }

        public override string ToString()
        {
            return base.ToString() + $"({Value})";
        }

    }
}