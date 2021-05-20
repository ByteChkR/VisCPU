using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

    public class ForLineDirective : VPPDirective
    {

        public ForLineDirective() : base("#for_lines", false)
        {
        }

        public override string Resolve(List<VPPMakro> makros, VPPTextParser parser, string[] args)
        {
            string block = ParseBlock(parser);

            VPPMakro tempMakro = new VPPTextMakro(
                                                  args[1],
                                                  block,
                                                  new List<VPPMakroParameter>
                                                  {
                                                      new VPPMakroParameter { Name = args[1] }
                                                  }
                                                 );

            makros.Add(tempMakro);
            string[] lines = File.ReadAllLines(args[0]);

            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(tempMakro.GenerateValue(new[] { line }));
            }
            makros.Remove(tempMakro);

            return sb.ToString();
        }

    }

    public class ForKeyDirective : VPPDirective
    {

        public ForKeyDirective() : base("#for_keys", false)
        {
        }

        public override string Resolve(List<VPPMakro> makros, VPPTextParser parser, string[] args)
        {
            string block = ParseBlock(parser);

            VPPMakro tempMakro = new VPPTextMakro(
                                                  args[1],
                                                  block,
                                                  new List<VPPMakroParameter>
                                                  {
                                                      new VPPMakroParameter { Name = args[1] }
                                                  }
                                                 );

            makros.Add(tempMakro);
            string[] lines = File.ReadAllLines(args[0]);

            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                string[] kvp = line.Split(new[] { '=' });

                sb.AppendLine(tempMakro.GenerateValue(new[] { kvp[0] }));
            }
            makros.Remove(tempMakro);

            string s = sb.ToString();
            return s;
        }

    }

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
