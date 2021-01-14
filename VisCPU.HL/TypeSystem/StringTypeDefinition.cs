namespace VisCPU.HL.TypeSystem
{

    public class StringTypeDefinition : HLTypeDefinition
    {

        #region Public

        public StringTypeDefinition() : base( HLCompilation.STRING_TYPE )
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion

    }

}
