using VisCPU.HL.Namespaces;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.TypeSystem
{

    public class StringTypeDefinition : HlTypeDefinition
    {
        #region Public

        public StringTypeDefinition( HlNamespace root ) : base(
            root,
            HLBaseTypeNames.s_StringTypeName,
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
