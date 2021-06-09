using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VPP.Importer.Directives
{

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
    public class ForListDirective : VPPDirective
    {

        public ForListDirective() : base("#for_list", true)
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
            string[] lines = args[0].Split(new []{ ' ', '\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                if(!string.IsNullOrWhiteSpace(line))
                sb.AppendLine(tempMakro.GenerateValue(new[] { line.Trim() }));
            }
            makros.Remove(tempMakro);

            return sb.ToString();
        }

    }

}