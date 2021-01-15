using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ListPackagesSubSystem : ConsoleSubsystem
    {

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            string searchStr = args.FirstOrDefault();
            foreach ( KeyValuePair < string, ModuleManager > keyValuePair in ModuleResolver.GetManagers() )
            {
                Log( $"{keyValuePair.Key} : {keyValuePair.Value.ModuleRoot}" );

                foreach ( ModulePackage modulePackage in keyValuePair.Value.GetPackages() )
                {
                    if(searchStr != null && !modulePackage.ModuleName.StartsWith(searchStr))
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

        public override void Help()
        {
            Log("vis origins packages <searchStr>");
        }
        #endregion

    }

}
