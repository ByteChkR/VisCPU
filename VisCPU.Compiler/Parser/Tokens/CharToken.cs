using System;
using System.Text.RegularExpressions;

using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Parser.Tokens
{

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