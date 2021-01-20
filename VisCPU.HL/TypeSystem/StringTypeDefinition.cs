namespace VisCPU.HL.TypeSystem
{

    public class StringTypeDefinition : HlTypeDefinition
    {
        #region Public

        public StringTypeDefinition() : base( HlCompilation.StringType )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion
    }

}
