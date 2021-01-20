using System.Collections.Generic;
using System.Linq;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{

    public abstract class HLMemberDefinition : IHlTypeSystemInstance
    {
        public bool IsPublic { get; }

        public bool IsStatic { get; }

        public bool IsConstant { get; }

        public string Name { get; }

        public int SourceIndex { get; }

        public abstract HLTokenType Type { get; }

        #region Public

        public abstract uint GetSize();

        public List < IHlToken > GetChildren()
        {
            return null;
        }

        #endregion

        #region Protected

        protected HLMemberDefinition( string name, IHlToken[] mods )
        {
            Name = name;

            IsConstant = mods.Any( x => x.Type == HLTokenType.OpConstMod );
            IsStatic = mods.Any( x => x.Type == HLTokenType.OpStaticMod );
            IsPublic = mods.Any( x => x.Type == HLTokenType.OpPublicMod );
        }

        #endregion
    }

}
