using System;
using System.IO;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.Console
{
    [Serializable]
    public class ConsoleOutInterfaceSettings
    {
        [Argument(Name = "console:out.pin.read")]
        public uint InterfacePresentPin = 0xFFFF1000;

        [Argument(Name = "console:out.pin.write.num")]
        public uint WriteNumOutputAddress = 0xFFFF1002;

        [Argument(Name = "console:out.pin.write.char")]
        public uint WriteOutputAddress = 0xFFFF1001;

        [Argument(Name = "console:out.pin.clear")]
        public uint InterfaceClearPin = 0xFFFF1005;

        #region Private

        static ConsoleOutInterfaceSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                new JSONSettingsLoader(),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "./config/console/out.json"),
                new ConsoleOutInterfaceSettings()
            );
        }

        #endregion

        #region Public

        public static ConsoleOutInterfaceSettings Create()
        {
            return SettingsSystem.GetSettings<ConsoleOutInterfaceSettings>();
        }

        #endregion
    }
}