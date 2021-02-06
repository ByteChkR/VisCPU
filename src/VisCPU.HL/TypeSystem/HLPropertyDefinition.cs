using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{
    public class HlPropertyDefinition : HlMemberDefinition
    {

        public HlTypeDefinition PropertyType { get; }

        public override HlTokenType Type => HlTokenType.OpPropertyDefinition;

        #region Public

        public HlPropertyDefinition( string name, HlTypeDefinition type, IHlToken[] mods ) : base( name, mods )
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
