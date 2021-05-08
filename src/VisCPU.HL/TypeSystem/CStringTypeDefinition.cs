using VisCPU.HL.Namespaces;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.TypeSystem
{

    public class CStringTypeDefinition : HlTypeDefinition
    {
        #region Public

        public CStringTypeDefinition( HlNamespace ns ) : base(
            ns,
            HLBaseTypeNames.s_CStringTypeName,
            true,
            false,
            false
        )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion
    }

}
