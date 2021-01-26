using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.TypeSystem
{

    public class FloatTypeDefinition : HlTypeDefinition
    {
        #region Public

        public FloatTypeDefinition() : base(HLBaseTypeNames.s_FloatTypeName)
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion
    }

}