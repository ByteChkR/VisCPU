using System.Collections.Generic;

using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{

    public abstract class HLMemberDefinition : IHLTypeSystemInstance
    {

        public string Name { get; }

        public int SourceIndex { get; }

        public abstract HLTokenType Type { get; }

        #region Public

        public abstract uint GetSize();

        public List < IHLToken > GetChildren()
        {
            return null;
        }

        #endregion

        #region Protected

        protected HLMemberDefinition( string name )
        {
            Name = name;
        }

        #endregion

    }

}
