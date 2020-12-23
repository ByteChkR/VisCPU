namespace VisCPU.HL.TypeSystem
{
    public class VarTypeDefinition : HLTypeDefinition
    {
        public VarTypeDefinition() : base("var")
        {
        }

        public override uint GetSize()
        {
            return 1;
        }
    }
}