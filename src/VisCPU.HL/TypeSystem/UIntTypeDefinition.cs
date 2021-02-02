using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.TypeSystem
{

    public class UIntTypeDefinition : HlTypeDefinition
    {
        #region Public

        public UIntTypeDefinition() : base( HLBaseTypeNames.s_UintTypeName )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion
    }

}
