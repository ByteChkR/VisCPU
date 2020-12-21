using VisCPU.HL.Parser;

namespace VisCPU.HL.TypeSystem
{
    public class HLFunctionDefinition : HLMemberDefinition
    {
        public HLTypeDefinition ReturnType { get; }
        public HLTypeDefinition[] ParameterTypes { get; }

        public override uint GetSize()
        {
            return 0;
        }

        public override HLTokenType Type=> HLTokenType.OpFunctionDefinition;

        public HLFunctionDefinition(
            string name,
            HLTypeDefinition returnType,
            HLTypeDefinition[] parameters ) : base( name )
        {
            ReturnType = returnType;
            ParameterTypes = parameters;
        }
    }

}