using VisCPU.HL.Parser;

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

}