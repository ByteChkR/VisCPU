using System;
using System.IO;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.Console
{

    [Serializable]
    public class ConsoleInInterfaceSettings
    {

        [field: Argument( Name = "console:in.pin.present" )]
        public uint InterfacePresentPin { get; set; } = 0xFFFF1003;

        [field: Argument( Name = "console:in.pin.read" )]
        public uint ReadInputAddress { get; set; } = 0xFFFF1004;
        

        #region Private

        static ConsoleInInterfaceSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                                                 new JSONSettingsLoader(),
                                                 Path.Combine(
                                                              AppDomain.CurrentDomain.BaseDirectory,
                                                              "./config/console/in.json"
                                                             ),
                                                 new ConsoleInInterfaceSettings()
                                                );
        }

        #endregion

    }

}
