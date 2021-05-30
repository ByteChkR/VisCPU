using System;
using System.Collections.Generic;
using System.IO;

namespace VPP.Importer
{

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