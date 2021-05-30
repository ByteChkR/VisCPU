using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VPP.Importer.Directives
{

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

}