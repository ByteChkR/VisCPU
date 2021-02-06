using System.Collections.Generic;

using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{

    public class HlExternalFunctionDefinition: HlFunctionDefinition
    {

        public readonly string TranslatedFunctionName;
        public HlExternalFunctionDefinition( string name, string translated, List <IHlToken> mods) : base( name, mods.ToArray())
        {
            TranslatedFunctionName = translated;
        }


    }

}