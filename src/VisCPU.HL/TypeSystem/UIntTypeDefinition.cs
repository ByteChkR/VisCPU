using VisCPU.HL.Namespaces;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.TypeSystem
{

    public class UIntTypeDefinition : HlTypeDefinition
    {

        #region Public

        public UIntTypeDefinition( HlNamespace root ) : base( root, HLBaseTypeNames.s_UintTypeName, true, false, true )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion

    }

}
