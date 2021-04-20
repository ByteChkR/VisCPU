using System.Collections.Generic;

namespace VPP.Importer
{

    public class VPPMakro
    {

        public string Name;
        public string Value;
        public List<VPPMakroParameter> Parameters;

        public string GenerateValue(string[] args)
        {
            if (Parameters.Count == 0)
            {
                return Value;
            }

            string v = Value;

            VPPTextParser parser = new VPPTextParser(v);

            for (int i = 0; i < Parameters.Count; i++)
            {
                ApplyParameter(parser, Parameters[i], args[i]);
            }
            return parser.ToString();
        }

        private void ApplyParameter(VPPTextParser parser, VPPMakroParameter p, string v)
        {
            parser.SetPosition(0);
            int idx;
            while ((idx = parser.Seek(p.Name)) != -1)
            {
                if (parser.IsValidPreWordCharacter(idx - 1) && parser.IsValidPostWordCharacter(idx+p.Name.Length))
                {
                    parser.Remove(p.Name.Length);
                    parser.Insert(v);
                }
                else
                {
                    parser.Eat();
                }

            }

        }

    }

}