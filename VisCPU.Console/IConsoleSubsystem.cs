using System.Collections.Generic;

using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace viscc
{

    public abstract class ConsoleSubsystem : VisBase
    {

        public abstract void Run( IEnumerable < string > args );

        protected override LoggerSystems SubSystem => LoggerSystems.Console;

    }
}