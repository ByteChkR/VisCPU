using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{

    public class HlFunctionDefinition : HlMemberDefinition
    {

        public HlTypeDefinition ReturnType { get; }

        public HlTypeDefinition[] ParameterTypes { get; }

        public override HlTokenType Type => HlTokenType.OpFunctionDefinition;

        #region Public

        public HlFunctionDefinition(
            string name,
            HlTypeDefinition returnType,
            HlTypeDefinition[] parameters,
            IHlToken[] mods) : base(name, mods)
        {
            ReturnType = returnType;
            ParameterTypes = parameters;
        }

        public HlFunctionDefinition(
            string name,
            IHlToken[] mods) : base(name, mods)
        {
            ReturnType = new UIntTypeDefinition();
        }

        public override uint GetSize()
        {
            return (IsVirtual|| IsAbstract) ? 1u:0u;
        }

        #endregion

    }

}
