namespace VisCPU.HL.TypeSystem
{
    public class VarTypeDefinition : HLTypeDefinition
    {

        public override uint GetSize()
        {
            return 1;
        }

        public VarTypeDefinition() : base( "var" )
        {
        }

    }
}