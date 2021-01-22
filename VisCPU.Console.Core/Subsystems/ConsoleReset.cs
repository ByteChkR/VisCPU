using System;
using System.Collections.Generic;
using System.IO;
using VisCPU.Utility;

namespace VisCPU.Console.Core.Subsystems
{

    public class ConsoleReset : ConsoleSubsystem
    {
        #region Public

        public override void Help()
        {
        }

        public override void Run( IEnumerable < string > args )
        {
            string configDir = Path.Combine( UnityIsAPieceOfShitHelper.AppRoot, "config" );
            string cacheDir = Path.Combine(UnityIsAPieceOfShitHelper.AppRoot, "cache" );

            if ( Directory.Exists( configDir ) )
            {
                Directory.Delete( configDir, true );
            }

            if ( Directory.Exists( cacheDir ) )
            {
                Directory.Delete( cacheDir, true );
            }
        }

        #endregion
    }

}
