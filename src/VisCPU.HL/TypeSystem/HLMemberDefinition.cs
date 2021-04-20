using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{

    public abstract class HlMemberDefinition : IHlTypeSystemInstance
    {

        public bool IsPublic { get; }

        public bool IsStatic { get; }

        public bool IsConstant { get; }

        public bool IsVirtual { get; }

        public bool IsOverride { get; }

        public bool IsAbstract { get; }

        public string Name { get; }

        public int SourceIndex { get; }

        public abstract HlTokenType Type { get; }

        #region Public

        public abstract uint GetSize();

        public List < IHlToken > GetChildren()
        {
            return null;
        }

        #endregion

        #region Protected

        protected HlMemberDefinition( string name, IHlToken[] mods )
        {
            Name = name;

            IsConstant = mods.Any( x => x.Type == HlTokenType.OpConstMod );
            IsStatic = mods.Any( x => x.Type == HlTokenType.OpStaticMod );
            IsPublic = mods.Any( x => x.Type == HlTokenType.OpPublicMod );
            IsVirtual = mods.Any( x => x.Type == HlTokenType.OpVirtualMod );
            IsOverride = mods.Any( x => x.Type == HlTokenType.OpOverrideMod );
            IsAbstract = mods.Any( x => x.Type == HlTokenType.OpAbstractMod );
        }

        #endregion

    }

}
