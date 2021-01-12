using System.Collections.Generic;

using VisCPU.Console.Core.Settings;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ListLocalPackagesSubSystem : ConsoleSubsystem
    {

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public override void Run( IEnumerable < string > args )
        {

            foreach ( KeyValuePair < string, ModuleManager > keyValuePair in ModuleResolver.GetManagers() )
            {
                Log( $"{keyValuePair.Key} : {keyValuePair.Value.ModuleRoot}" );

                foreach ( ModulePackage modulePackage in keyValuePair.Value.GetPackages() )
                {
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
