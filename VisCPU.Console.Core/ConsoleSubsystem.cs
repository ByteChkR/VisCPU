using System.Collections.Generic;
using VisCPU.Utility;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core
{
    public abstract class ConsoleSubsystem : VisBase
    {
        protected override LoggerSystems SubSystem => LoggerSystems.Console;

        #region Public

        public abstract void Run(IEnumerable<string> args);

        #endregion
    }
}