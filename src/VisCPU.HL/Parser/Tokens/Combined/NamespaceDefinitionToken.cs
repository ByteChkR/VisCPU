using VisCPU.HL.Namespaces;

namespace VisCPU.HL.Parser.Tokens.Combined
{

    public class NamespaceDefinitionToken : CombinedToken
    {
        public HlNamespace Namespace { get; }

        #region Public

        public NamespaceDefinitionToken( HlNamespace ns, IHlToken[] subtokens, int start ) : base(
            HlTokenType.OpNamespaceDefinition,
            subtokens,
            start )
        {
            Namespace = ns;
        }

        #endregion
    }

}