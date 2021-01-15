namespace VisCPU.HL.Parser.Tokens.Combined
{

    public class FunctionDefinitionToken : CombinedToken
    {

        public readonly IHlToken[] Arguments;
        public readonly IHlToken[] Block;

        public readonly IHlToken FunctionName;
        public readonly IHlToken FunctionReturnType;
        public readonly IHlToken[] Mods;

        #region Public

        public FunctionDefinitionToken(
            IHlToken name,
            IHlToken retType,
            IHlToken[] args,
            IHlToken[] mods,
            IHlToken[] subtokens,
            int start ) : base(
                               HLTokenType.OpFunctionDefinition,
                               subtokens,
                               start
                              )
        {
            FunctionName = name;
            FunctionReturnType = retType;
            Mods = mods;
            Arguments = args;
            Block = subtokens;
        }

        #endregion

    }

}
