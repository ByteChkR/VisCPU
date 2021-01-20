﻿using System.Collections.Generic;
using VisCPU.ProjectSystem.Resolvers;
using VisCPU.Utility.Settings;

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
