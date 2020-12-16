﻿using System;

using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Parser.Tokens
{

    public class DecToken : ValueToken
    {

        public DecToken(string originalText, int start, int length) : base(originalText, start, length)
        {
        }

        public override uint Value
        {
            get
            {
                string val = GetValue();
                if (uint.TryParse(val, out uint hexVal))
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