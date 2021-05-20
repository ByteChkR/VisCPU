using System;
using System.Collections.Generic;
using System.IO;

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

    public class VPPGetValueMakro : VPPMakro
    {
        public VPPGetValueMakro() : base("vpp_get_value", new List<VPPMakroParameter> { new() { Name = "file" }, new VPPMakroParameter{Name = "key"} })
        {

        }

        public override string GenerateValue(string[] args)
        {
            string[] lines = File.ReadAllLines( args[0] );

            foreach ( string line in lines )
            {
                string[] kvp = line.Split( '=' );

                if ( args[1] == kvp[0] )
                {
                    return kvp[1];
                }
            }

            throw new Exception( $"Key '{args[1]}' not found in file '{args[0]}'" );
        }

    }

}