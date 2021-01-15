using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace VisCPU.HL.Modules.Data
{

    public class BuildJob
    {

        public string JobName { get; set; }

        public string BuildJobRunner { get; set; }

        public Dictionary < string, string > Arguments { get; } = new Dictionary < string, string >();

        #region Public

        public static BuildJob Deserialize( string data )
        {
            return JsonConvert.DeserializeObject < BuildJob >( data );
        }

        public static BuildJob Load( string path )
        {
            return Deserialize( File.ReadAllText( path ) );
        }

        public static void Save( string path, BuildJob config )
        {
            File.WriteAllText( path, Serialize( config ) );
        }

        public static string Serialize( BuildJob config )
        {
            return JsonConvert.SerializeObject(
                                               config,
                                               Formatting.Indented
                                              );
        }

        #endregion

    }

}
