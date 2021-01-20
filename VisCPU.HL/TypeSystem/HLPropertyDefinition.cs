using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{

    public class HLPropertyDefinition : HLMemberDefinition
    {
        public HLTypeDefinition PropertyType { get; }

        public override HLTokenType Type => HLTokenType.OpPropertyDefinition;

        #region Public

        public HLPropertyDefinition( string name, HLTypeDefinition type, IHlToken[] mods ) : base( name, mods )
        {
            PropertyType = type;
        }

        public override uint GetSize()
        {
            return PropertyType.GetSize();
        }

        #endregion
    }

}
