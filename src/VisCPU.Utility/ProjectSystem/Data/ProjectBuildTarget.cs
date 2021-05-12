using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace VisCPU.Utility.ProjectSystem.Data
{

    public class ProjectBuildTarget
    {

        public string TargetName { get; set; }

        public string[] DependsOn { get; set; } = new string[0];

        public List < ProjectBuildJob > Jobs { get; set; } = new List < ProjectBuildJob >();

        #region Public

        public static ProjectBuildTarget Deserialize( string data )
        {
            return JsonConvert.DeserializeObject < ProjectBuildTarget >( data );
        }

        public static ProjectBuildTarget Load( string path )
        {
            return Deserialize( File.ReadAllText( path ) );
        }

        public static void Save( string path, ProjectBuildTarget config )
        {
            File.WriteAllText( path, Serialize( config ) );
        }

        public static string Serialize( ProjectBuildTarget config )
        {
            return JsonConvert.SerializeObject(
                                               config,
                                               Formatting.Indented
                                              );
        }

        #endregion

    }

}
