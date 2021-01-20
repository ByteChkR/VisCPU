namespace VisCPU.HL.TypeSystem
{

    public class VarTypeDefinition : HlTypeDefinition
    {
        #region Public

        public VarTypeDefinition() : base( HlCompilation.ValType )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion
    }

}
