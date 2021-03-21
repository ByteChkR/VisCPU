using VisCPU.HL.Namespaces;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.TypeSystem
{

    public class FloatTypeDefinition : HlTypeDefinition
    {

        #region Public

        public FloatTypeDefinition(HlNamespace root) : base(root, HLBaseTypeNames.s_FloatTypeName, true, false, true )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion

    }

}
