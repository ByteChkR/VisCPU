﻿using System.Collections.Generic;
using System.Linq;
using VisCPU.ProjectSystem.Resolvers;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    public class RemoveOriginSubSystem : ConsoleSubsystem
    {
        #region Public

        public static void RemoveOrigin( string name )
        {
            ProjectResolverSettings s = SettingsManager.GetSettings < ProjectResolverSettings >();
            s.ModuleOrigins.Remove( name );
            SettingsManager.SaveSettings( s );
        }

        public override void Help()
        {
            Log( "vis origin remove <name>" );
        }

        public override void Run( IEnumerable < string > args )
        {
            RemoveOrigin( args.First() );
        }

        #endregion
    }

}