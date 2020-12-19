using VisCPU.HL.Parser;

namespace VisCPU.HL.TypeSystem
{
    public class HLPropertyDefinition : HLMemberDefinition
    {
        public HLTypeDefinition PropertyType { get; }
        public HLPropertyDefinition( string name, HLTypeDefinition type ) : base( name )
        {
            PropertyType = type;
        }

        public override uint GetSize()
        {
            return PropertyType.GetSize();
        }

        public override HLTokenType Type => HLTokenType.OpPropertyDefinition;

    }
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