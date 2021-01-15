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
        public uint InterfacePresentPin { get; } = 0xFFFF1003;

        [field: Argument( Name = "console:in.pin.read" )]
        public uint ReadInputAddress { get; } = 0xFFFF1004;

        #region Public

        public static ConsoleInInterfaceSettings Create()
        {
            return SettingsSystem.GetSettings < ConsoleInInterfaceSettings >();
        }

        #endregion

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
