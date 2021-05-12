using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace VisCPU.Utility.ProjectSystem.Data
{

    public class ProjectBuildJob
    {

        public string JobName { get; set; }

        public string BuildJobRunner { get; set; }

        public Dictionary < string, string > Arguments { get; } = new Dictionary < string, string >();

        #region Public

        public static ProjectBuildJob Deserialize( string data )
        {
            return JsonConvert.DeserializeObject < ProjectBuildJob >( data );
        }

        public static ProjectBuildJob Load( string path )
        {
            return Deserialize( File.ReadAllText( path ) );
        }

        public static void Save( string path, ProjectBuildJob config )
        {
            File.WriteAllText( path, Serialize( config ) );
        }

        public static string Serialize( ProjectBuildJob config )
        {
            return JsonConvert.SerializeObject(
                                               config,
                                               Formatting.Indented
                                              );
        }

        #endregion

    }

}
