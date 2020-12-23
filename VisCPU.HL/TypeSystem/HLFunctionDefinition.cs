using VisCPU.HL.Parser;

namespace VisCPU.HL.TypeSystem
{
    public class HLFunctionDefinition : HLMemberDefinition
    {
        public HLFunctionDefinition(
            string name,
            HLTypeDefinition returnType,
            HLTypeDefinition[] parameters) : base(name)
        {
            ReturnType = returnType;
            ParameterTypes = parameters;
        }

        public HLTypeDefinition ReturnType { get; }
        public HLTypeDefinition[] ParameterTypes { get; }

        public override HLTokenType Type => HLTokenType.OpFunctionDefinition;

        public override uint GetSize()
        {
            return 0;
        }
    }
}