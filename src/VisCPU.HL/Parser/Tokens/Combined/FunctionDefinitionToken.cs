using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.Parser.Tokens.Combined
{

    public class FunctionDefinitionToken : CombinedToken
    {

        public IHlToken[] Arguments { get; }

        public IHlToken[] Block { get; }

        public IHlToken FunctionName { get; }

        public IHlToken FunctionReturnType { get; }

        public IHlToken[] Mods { get; }

        public HlTypeDefinition Parent { get; }

        public HlFunctionType FunctionType { get; }

        #region Public

        public FunctionDefinitionToken(
            IHlToken name,
            IHlToken retType,
            IHlToken[] args,
            IHlToken[] mods,
            IHlToken[] subtokens,
            int start,
            HlTypeDefinition parent = null,
            HlFunctionType functionType = HlFunctionType.Function ) : base(
                                                                           HlTokenType.OpFunctionDefinition,
                                                                           subtokens,
                                                                           start
                                                                          )
        {
            FunctionType = functionType;
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
