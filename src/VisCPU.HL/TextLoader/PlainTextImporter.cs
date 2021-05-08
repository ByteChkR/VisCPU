namespace VisCPU.HL.TextLoader
{

    public class PlainTextImporter : TextImporter
    {
        public override string Name => nameof( PlainTextImporter );

        #region Public

        public override string Import( string text, string rootDir )
        {
            return text;
        }

        #endregion
    }

}
