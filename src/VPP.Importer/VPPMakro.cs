using System.Collections.Generic;

namespace VPP.Importer
{

    public class VPPUniqueMakro : VPPMakro
    {

        private static int s_UniqueId = 0;
        private static readonly Dictionary < string, string > s_UniqueMap = new ();
        public VPPUniqueMakro() : base("vpp_unique", new List<VPPMakroParameter> { new() { Name = "var_prefix" } })
        {

        }

        public override string GenerateValue( string[] args )
        {
            if (s_UniqueMap.ContainsKey( args[0] ) )
                return s_UniqueMap[args[0]];
            return s_UniqueMap[args[0]] = $"{args[0]}_{s_UniqueId++}";
        }

    }

    public class VPPTextMakro : VPPMakro
    {

        private string Value;


        public VPPTextMakro(string name, string value, List<VPPMakroParameter> parameters) : base(name, parameters)
        {
            Value = value;
        }

        public override string GenerateValue(string[] args)
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
                if (parser.IsValidPreWordCharacter(idx - 1) &&
                    parser.IsValidPostWordCharacter(idx + p.Name.Length))
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

    public abstract class VPPMakro
    {

        public readonly string Name;
        public readonly List<VPPMakroParameter> Parameters;

        protected VPPMakro(string name, List<VPPMakroParameter> parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        #region Public

        public abstract string GenerateValue(string[] args);
        #endregion

        #region Private


        #endregion

    }

}
