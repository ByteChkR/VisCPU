using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace VisCPU.HL.Modules.Data
{

    public class ProjectBuildTarget
    {

        public string TargetName;
        public string[] DependsOn = new string[0];
        public List < BuildJob > Jobs = new List < BuildJob >();

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
