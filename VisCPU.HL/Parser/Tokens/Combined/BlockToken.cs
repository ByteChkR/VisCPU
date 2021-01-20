namespace VisCPU.HL.Parser.Tokens.Combined
{

    public class BlockToken : CombinedToken
    {
        #region Public

        public BlockToken( IHlToken[] subtokens, int start ) : base( HLTokenType.OpBlockToken, subtokens, start )
        {
        }

        #endregion
    }

}
