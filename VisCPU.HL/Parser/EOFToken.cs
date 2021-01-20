using System.Collections.Generic;
using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.Parser
{

    public class EOFToken : IHlToken
    {
        public HLTokenType Type => HLTokenType.Eof;

        public int SourceIndex { get; }

        #region Public

        public List < IHlToken > GetChildren()
        {
            return new List < IHlToken >();
        }

        #endregion
    }

}
