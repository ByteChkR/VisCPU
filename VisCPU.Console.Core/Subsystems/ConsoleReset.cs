using System;
using System.Collections.Generic;
using System.IO;

namespace VisCPU.Console.Core.Subsystems
{

    public class ConsoleReset : ConsoleSubsystem
    {

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            string configDir = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config" );
            string cacheDir = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "cache" );

            if ( Directory.Exists( configDir ) )
            {
                Directory.Delete( configDir, true );
            }

            if ( Directory.Exists( cacheDir ) )
            {
                Directory.Delete( cacheDir, true );
            }
        }

        public override void Help()
        {
        }

        #endregion

    }

}
