using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.TypeSystem
{

    public class FloatTypeDefinition : HlTypeDefinition
    {

        #region Public

        public FloatTypeDefinition() : base( HLBaseTypeNames.s_FloatTypeName, true, true )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion

    }

}
