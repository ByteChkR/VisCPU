using System.Collections.Generic;

namespace VPP.Importer.Directives
{

    public abstract class VPPDirective
    {

        public readonly string DirectiveName;

        public readonly bool ResolveParameters;

        protected VPPDirective( string name, bool resolveParameters )
        {
            DirectiveName = name;
            ResolveParameters = resolveParameters;
        }

        protected static string ParseBlock(VPPTextParser parser)
        {
            parser.EatWhiteSpace();
            int pStart = parser.Eat('{');
            int end = parser.FindClosing('{', '}');
            parser.SetPosition(pStart + 1);
            string block = parser.Get(end - pStart - 1).Trim();
            parser.SetPosition(pStart);
            parser.Remove(end + 1 - pStart);

            return block;
        }

        public abstract string Resolve(List <VPPMakro> makros, VPPTextParser parser , string[] args);
    }

}
