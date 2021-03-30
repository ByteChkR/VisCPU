using System.Collections.Generic;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.ProjectSystem.Resolvers;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    public class ListOriginSubSystem : ConsoleSubsystem
    {
        #region Public

        public override void Help()
        {
        }

        public override void Run( IEnumerable < string > args )
        {
            ProjectResolverSettings s = SettingsManager.GetSettings < ProjectResolverSettings >();

            foreach ( KeyValuePair < string, string > keyValuePair in s.ModuleOrigins )
            {
                Log( $"{keyValuePair.Key} : {keyValuePair.Value}" );
            }
        }

        #endregion
    }

}
