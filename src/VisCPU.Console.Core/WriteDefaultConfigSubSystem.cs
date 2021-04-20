using System.Collections.Generic;

using VisCPU.Console.Core.Settings;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility.IO.Settings;

namespace VisCPU.Console.Core
{

    public class WriteDefaultConfigSubSystem : ConsoleSubsystem
    {

        #region Public

        public override void Help()
        {
        }

        public override void Run( IEnumerable < string > args )
        {
            SettingsManager.GetSettings < CliSettings >();
            SettingsManager.GetSettings < CpuSettings >();
            SettingsManager.GetSettings < MemorySettings >();
            SettingsManager.GetSettings < MemoryBusSettings >();
        }

        #endregion

    }

}
