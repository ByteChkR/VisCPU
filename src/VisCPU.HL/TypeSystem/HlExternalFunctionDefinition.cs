using System.Collections.Generic;

using VisCPU.HL.Namespaces;
using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{

    public class HlExternalFunctionDefinition : HlFunctionDefinition
    {

        public readonly string TranslatedFunctionName;

        #region Public

        public HlExternalFunctionDefinition(HlTypeSystem ts, HlNamespace root, string name, string translated, List < IHlToken > mods ) : base(ts,root,
             name,
             mods.ToArray()
            )
        {
            TranslatedFunctionName = translated;
        }

        #endregion

    }

}
