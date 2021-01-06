using System;
using System.Collections.Generic;

using VisCPU.Console.Core.Settings;
using VisCPU.Console.Core.Subsystems.Origins.Events;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    public class RefreshOriginSubSystem : ConsoleSubsystem
    {

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            OriginSettings s = SettingsSystem.GetSettings < OriginSettings >();

            foreach ( KeyValuePair < string, string > keyValuePair in s.origins )
            {
                Uri url = new Uri( keyValuePair.Value, UriKind.RelativeOrAbsolute );

                if ( url.Scheme == "file" )
                {
                    LocalModuleManager lm = new LocalModuleManager( url.OriginalString );

                    foreach ( ModulePackage modulePackage in lm.GetPackages() )
                    {
                        if ( ModuleResolver.Manager.HasPackage( modulePackage.ModuleName ) )
                        {
                            foreach ( string moduleVersion in modulePackage.ModuleVersions )
                            {
                                ModuleTarget t = modulePackage.GetInstallTarget( moduleVersion );
                                ModuleResolver.Manager.AddPackage( t, lm.GetTargetDataPath( t ) );
                            }
                        }
                        else
                        {
                            ModulePackage existing = ModuleResolver.Manager.GetPackage( modulePackage.ModuleName );

                            foreach ( string moduleVersion in modulePackage.ModuleVersions )
                            {
                                if ( !existing.HasTarget( moduleVersion ) )
                                {
                                    ModuleTarget t = modulePackage.GetInstallTarget( moduleVersion );
                                    ModuleResolver.Manager.AddPackage( t, lm.GetTargetDataPath( t ) );
                                }
                            }
                        }
                    }

                    continue;
                }

                EventManager < ErrorEvent >.SendEvent( new OriginUrlSchemeNotSupportedEvent( url.Scheme ) );
            }
        }

        #endregion

    }

}
