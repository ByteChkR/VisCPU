using System.Collections.Generic;
using System.Linq;

namespace VPP.Importer.Directives
{

    public class IfDirective : VPPDirective
    {

        public IfDirective() : base("#if", true)
        {
        }

        public override string Resolve(List<VPPMakro> makros, VPPTextParser parser, string[] args)
        {
            string block = ParseBlock(parser);


            return args[0] == "1" || makros.Any( x => x.Name == args[0] ) ? block : "";
        }

    }

}