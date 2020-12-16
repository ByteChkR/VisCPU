using System;

using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals
{

    [Serializable]
    public class ConsoleInInterfaceSettings
    {

        [Argument(Name="console:in.pin.present")]
        public uint InterfacePresentPin = 0xFFFF1003;
        [Argument(Name = "console:in.pin.read")]
        public uint ReadInputAddress = 0xFFFF1004;
        static ConsoleInInterfaceSettings()
        {
            Settings.RegisterDefaultLoader(new JSONSettingsLoader(), "./config/console/in.json", new ConsoleInInterfaceSettings());
        }

        public static ConsoleInInterfaceSettings Create()
        {
            return Settings.GetSettings<ConsoleInInterfaceSettings>();
        }
    }

}