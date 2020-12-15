namespace VisCPU.HL.Parser.Tokens.Combined
{
    public class BlockToken : CombinedToken
    {

        public BlockToken(IHLToken[] subtokens, int start) : base(HLTokenType.OpBlockToken, subtokens, start)
        {
        }

    }
}