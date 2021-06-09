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

        private static string Sanitize( string str )
        {
            char[] arr = str.ToCharArray();

            for ( int i = 0; i < arr.Length; i++ )
            {
                if ( !char.IsLetterOrDigit( arr[i] ) )
                    arr[i] = '_';
            }

            return new string( arr );
        }

        public override string GenerateValue(string[] args)
        {
            string cleanStr = Sanitize(args[0]);
            
            if (s_UniqueMap.ContainsKey(cleanStr))
                return s_UniqueMap[cleanStr];
            return s_UniqueMap[cleanStr] = $"{cleanStr}_{s_UniqueId++}";
        }

    }

}