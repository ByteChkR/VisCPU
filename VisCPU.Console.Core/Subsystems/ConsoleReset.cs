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
            Directory.Delete( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config" ), true );
            Directory.Delete( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "cache" ), true );
        }

        #endregion

    }

}
