using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Peripherals.Console
{

    public class ConsoleInterfaceSettings
    {
        public uint InterfacePresentPin { get; set; } = 0xFFFF1020;

        public uint InterfaceClearPin { get; set; } = 0xFFFF1021;

        public uint WidthAddr { get; set; } = 0xFFFF1022;

        public uint HeightAddr { get; set; } = 0xFFFF1023;

        public uint CursorLeftAddr { get; set; } = 0xFFFF1024;

        public uint CursorTopAddr { get; set; } = 0xFFFF1025;

        public uint BackColorAddr { get; set; } = 0xFFFF1026;

        public uint ForeColorAddr { get; set; } = 0xFFFF1027;

        public uint ResetColorAddr { get; set; } = 0xFFFF1028;

        public uint BufWidthAddr { get; set; } = 0xFFFF1029;

        public uint BufHeightAddr { get; set; } = 0xFFFF102A;

        #region Public

        public bool Any( uint addr )
        {
            return addr == InterfacePresentPin ||
                   addr == WidthAddr ||
                   addr == HeightAddr ||
                   addr == CursorLeftAddr ||
                   addr == CursorTopAddr ||
                   addr == BackColorAddr ||
                   addr == ForeColorAddr ||
                   addr == ResetColorAddr ||
                   addr == BufHeightAddr ||
                   addr == BufWidthAddr ||
                   addr == InterfaceClearPin;
        }

        #endregion

        #region Private

        static ConsoleInterfaceSettings()
        {
            SettingsCategory coutCategory = Peripheral.PeripheralCategory.AddCategory( "console" );

            SettingsManager.RegisterDefaultLoader(
                new JsonSettingsLoader(),
                coutCategory,
                "management.json",
                new ConsoleInterfaceSettings()
            );
        }

        #endregion
    }

}
