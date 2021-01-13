﻿namespace VisCPU.HL.TypeSystem
{

    public class VarTypeDefinition : HLTypeDefinition
    {

        #region Public

        public VarTypeDefinition() : base(HLCompilation.VAL_TYPE)
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion

    }

    public class StringTypeDefinition : HLTypeDefinition
    {

        #region Public

        public StringTypeDefinition() : base(HLCompilation.STRING_TYPE)
        {
        }

        public override uint GetSize()
        {
            return 1;
        }

        #endregion

    }

}
