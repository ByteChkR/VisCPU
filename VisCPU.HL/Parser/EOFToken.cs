using System;
using System.Collections.Generic;

using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.Parser
{

    public class EOFToken : IHLToken
    {

        public HLTokenType Type => HLTokenType.EOF;

        public int SourceIndex { get; }

        #region Public

        public List < IHLToken > GetChildren()
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
