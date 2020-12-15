using System;
using System.Globalization;

using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Parser.Tokens
{
    public class HexToken : ValueToken
    {

        public HexToken(string originalText, int start, int length) : base(originalText, start, length)
        {
        }

        public override uint Value
        {
            get
            {
                string val = GetValue().Remove(0, 2);
                if (uint.TryParse(val, NumberStyles.HexNumber, null, out uint hexVal))
                {
                    return hexVal;
                }

                EventManager < ErrorEvent >.SendEvent( new InvalidDecValueEvent( val, Start ) );

                return 0;
            }
        }

        public override string ToString()
        {
            return base.ToString() + $"({Value})";
        }

    }
}