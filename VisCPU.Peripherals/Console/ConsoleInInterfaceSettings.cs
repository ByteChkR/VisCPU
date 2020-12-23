using System;
using System.IO;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.Console
{
    [Serializable]
    public class ConsoleInInterfaceSettings
    {
        [Argument(Name = "console:in.pin.present")]
        public uint InterfacePresentPin = 0xFFFF1003;

        [Argument(Name = "console:in.pin.read")]
        public uint ReadInputAddress = 0xFFFF1004;

        #region Private

        static ConsoleInInterfaceSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                new JSONSettingsLoader(),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "./config/console/in.json"),
                new ConsoleInInterfaceSettings()
            );
        }

        #endregion

        #region Public

        public static ConsoleInInterfaceSettings Create()
        {
            return SettingsSystem.GetSettings<ConsoleInInterfaceSettings>();
        }

        #endregion
    }
}