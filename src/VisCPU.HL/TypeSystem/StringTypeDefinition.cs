using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.TypeSystem
{

    public class StringTypeDefinition : HlTypeDefinition
    {
        #region Public

        public StringTypeDefinition() : base( HLBaseTypeNames.s_StringTypeName )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion
    }

}
