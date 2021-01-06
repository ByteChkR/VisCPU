using VisCPU.HL.Parser;

namespace VisCPU.HL.TypeSystem
{

    public class HLFunctionDefinition : HLMemberDefinition
    {

        public HLTypeDefinition ReturnType { get; }

        public HLTypeDefinition[] ParameterTypes { get; }

        public override HLTokenType Type => HLTokenType.OpFunctionDefinition;

        #region Public

        public HLFunctionDefinition(
            string name,
            HLTypeDefinition returnType,
            HLTypeDefinition[] parameters ) : base( name )
        {
            ReturnType = returnType;
            ParameterTypes = parameters;
        }

        public override uint GetSize()
        {
            return 0;
        }

        #endregion

    }

}
