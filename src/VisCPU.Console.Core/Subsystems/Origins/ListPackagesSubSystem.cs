using System.Collections.Generic;
using System.Linq;

using VisCPU.Utility.Logging;
using VisCPU.Utility.ProjectSystem.Data;
using VisCPU.Utility.ProjectSystem.Database;
using VisCPU.Utility.ProjectSystem.Resolvers;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    public class ListPackagesSubSystem : ConsoleSubsystem
    {

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public override void Help()
        {
            Log( "vis origins packages <searchStr>" );
        }

        public override void Run( IEnumerable < string > args )
        {
            string searchStr = args.FirstOrDefault();

            foreach ( KeyValuePair < string, ProjectDatabase > keyValuePair in ProjectResolver.GetManagers() )
            {
                Log( $"{keyValuePair.Key} : {keyValuePair.Value.ModuleRoot}" );

                foreach ( ProjectPackage modulePackage in keyValuePair.Value.GetPackages() )
                {
                    if ( searchStr != null && !modulePackage.ModuleName.StartsWith( searchStr ) )
                    {
                        continue;
                    }

                    Log( $"\t{modulePackage.ModuleName}" );

                    foreach ( string modulePackageModuleVersion in modulePackage.ModuleVersions )
                    {
                        Log( $"\t\t{modulePackageModuleVersion}" );
                    }
                }
            }
        }

        #endregion

    }

}
