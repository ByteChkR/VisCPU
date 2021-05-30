using System.Collections.Generic;

namespace VPP.Importer
{

    public class VPPUniqueMakro : VPPMakro
    {

        private static int s_UniqueId = 0;
        private static readonly Dictionary<string, string> s_UniqueMap = new();
        public VPPUniqueMakro() : base("vpp_unique", new List<VPPMakroParameter> { new() { Name = "var_prefix" } })
        {

        }

        public override string GenerateValue(string[] args)
        {
            if (s_UniqueMap.ContainsKey(args[0]))
                return s_UniqueMap[args[0]];
            return s_UniqueMap[args[0]] = $"{args[0]}_{s_UniqueId++}";
        }

    }

}