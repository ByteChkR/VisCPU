using System.Collections.Generic;

using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Console.Core
{

    public abstract class ConsoleSubsystem : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.Console;

        #region Public

        public abstract void Run( IEnumerable < string > args );
        public abstract void Help();

        #endregion

    }

}
