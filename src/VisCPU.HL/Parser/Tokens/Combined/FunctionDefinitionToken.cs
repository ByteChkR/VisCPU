using VisCPU.HL.Namespaces;
using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.Parser.Tokens.Combined
{

    public class NamespaceDefinitionToken:CombinedToken
    {
        public HlNamespace Namespace { get; }

        public NamespaceDefinitionToken(HlNamespace ns, IHlToken[] subtokens, int start ) : base( HlTokenType.OpNamespaceDefinition, subtokens, start )
        {
            Namespace = ns;
        }

    }

    public class FunctionDefinitionToken : CombinedToken
    {

        public IHlToken[] Arguments { get; }

        public IHlToken[] Block { get; }

        public IHlToken FunctionName { get; }

        public IHlToken FunctionReturnType { get; }

        public IHlToken[] Mods { get; }

        public HlTypeDefinition Parent { get; }

        public HlFunctionType Type { get; }

        #region Public

        public FunctionDefinitionToken(
            IHlToken name,
            IHlToken retType,
            IHlToken[] args,
            IHlToken[] mods,
            IHlToken[] subtokens,
            int start,
            HlTypeDefinition parent = null,
            HlFunctionType type = HlFunctionType.Function ) : base(
                                                                   HlTokenType.OpFunctionDefinition,
                                                                   subtokens,
                                                                   start
                                                                  )
        {
            Type = type;
            Parent = parent;
            FunctionName = name;
            FunctionReturnType = retType;
            Mods = mods;
            Arguments = args;
            Block = subtokens;
        }

        #endregion

    }

}
