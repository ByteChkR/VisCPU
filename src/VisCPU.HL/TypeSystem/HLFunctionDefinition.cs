﻿using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{

    public class HlExternalFunctionDefinition: HlMemberDefinition
    {

        public readonly string TranslatedFunctionName;
        public HlExternalFunctionDefinition( string name, string translated) : base( name, new []{new HlTextToken(HlTokenType.OpPublicMod, "public",0)} )
        {
            TranslatedFunctionName = translated;
        }

        public override HlTokenType Type => HlTokenType.OpFunctionDefinition;

        public override uint GetSize()
        {
            return 0;
        }

    }

    public class HlFunctionDefinition : HlMemberDefinition
    {

        public HlTypeDefinition ReturnType { get; }

        public HlTypeDefinition[] ParameterTypes { get; }

        public override HlTokenType Type => HlTokenType.OpFunctionDefinition;

        #region Public

        public HlFunctionDefinition(
            string name,
            HlTypeDefinition returnType,
            HlTypeDefinition[] parameters,
            IHlToken[] mods ) : base( name, mods )
        {
            ReturnType = returnType;
            ParameterTypes = parameters;
        }

        public override uint GetSize()
        {
            return 0;
        }

        #endregion

    }

}
