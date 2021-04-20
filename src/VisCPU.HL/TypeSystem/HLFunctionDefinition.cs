using VisCPU.HL.Namespaces;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;
using VisCPU.Utility.SharedBase;

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
            IHlToken[] mods ) : base( name, mods )
        {
            ReturnType = returnType;
            ParameterTypes = parameters;
        }

        public HlFunctionDefinition(
            HlTypeSystem ts,
            HlNamespace root,
            string name,
            IHlToken[] mods ) : base( name, mods )
        {
            ReturnType = ts.GetType( root, HLBaseTypeNames.s_UintTypeName );
        }

        public override uint GetSize()
        {
            return IsVirtual || IsAbstract ? 1u : 0u;
        }

        #endregion

    }

}
