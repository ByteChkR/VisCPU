using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.TypeSystem
{

    public class StringTypeDefinition : HlTypeDefinition
    {

        #region Public

        public StringTypeDefinition() : base( HLBaseTypeNames.s_StringTypeName, true, false )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion

    }

}
