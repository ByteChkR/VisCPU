namespace VisCPU.HL.Parser.Tokens.Combined
{
    public class FunctionDefinitionToken : CombinedToken
    {

        public readonly IHLToken[] Arguments;
        public readonly IHLToken[] Block;

        public readonly IHLToken FunctionName;
        public readonly IHLToken FunctionReturnType;
        public readonly IHLToken[] Mods;

        public FunctionDefinitionToken(
            IHLToken name, IHLToken retType, IHLToken[] args, IHLToken[] mods, IHLToken[] subtokens, int start) : base(
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

    }
}