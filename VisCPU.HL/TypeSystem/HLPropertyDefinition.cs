using VisCPU.HL.Parser;

namespace VisCPU.HL.TypeSystem
{

    public class HLPropertyDefinition : HLMemberDefinition
    {

        public HLTypeDefinition PropertyType { get; }

        public override HLTokenType Type => HLTokenType.OpPropertyDefinition;

        #region Public

        public HLPropertyDefinition( string name, HLTypeDefinition type ) : base( name )
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
