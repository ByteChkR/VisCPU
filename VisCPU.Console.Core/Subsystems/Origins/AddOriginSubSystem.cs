using System.Collections.Generic;
using System.Linq;

using VisCPU.ProjectSystem.Resolvers;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    public class AddOriginSubSystem : ConsoleSubsystem
    {

        #region Public

        public static void AddOrigin( string name, string uri )
        {
            ProjectResolver.ResolverSettings.ModuleOrigins[name] = uri;
            SettingsManager.SaveSettings( ProjectResolver.ResolverSettings );
            ProjectResolver.AddManager( name, uri );
        }

        public override void Help()
        {
            Log( "vis origin add <name> <uri>" );
        }

        public override void Run( IEnumerable < string > args )
        {
            string[] a = args.ToArray();
            string name = a[0];
            string url = a[1];
            AddOrigin( name, url );
        }

        #endregion

    }

}
