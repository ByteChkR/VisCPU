using VisCPU.HL.Namespaces;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.TypeSystem
{
    public class CStringTypeDefinition : HlTypeDefinition
    {
        public CStringTypeDefinition( HlNamespace ns) : base(
            ns,
            HLBaseTypeNames.s_CStringTypeName,
            true,
            false,
            false)
        {
        }


        public override uint GetSize()
        {
            return 1;
        }

    }

    public class StringTypeDefinition : HlTypeDefinition
    {
        #region Public

        public StringTypeDefinition( HlNamespace root ) : base(
            root,
            HLBaseTypeNames.s_StringTypeName,
            true,
            false,
            false )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion
    }

}
