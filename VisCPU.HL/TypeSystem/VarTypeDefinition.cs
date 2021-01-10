namespace VisCPU.HL.TypeSystem
{

    public class VarTypeDefinition : HLTypeDefinition
    {

        #region Public

        public VarTypeDefinition() : base( "var" )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion

    }

}
