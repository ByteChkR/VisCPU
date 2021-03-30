using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Peripherals.Console
{

    public class ConsoleInterfaceSettings
    {
        public uint InterfacePresentPin { get; set; } = 0xFFFF1006;

        public uint WidthAddr { get; set; } = 0xFFFF1007;

        public uint HeightAddr { get; set; } = 0xFFFF1008;

        public uint CursorLeftAddr { get; set; } = 0xFFFF1009;

        public uint CursorTopAddr { get; set; } = 0xFFFF100A;

        public uint BackColorAddr { get; set; } = 0xFFFF100B;

        public uint ForeColorAddr { get; set; } = 0xFFFF100C;

        public uint ResetColorAddr { get; set; } = 0xFFFF100D;

        public uint BufWidthAddr { get; set; } = 0xFFFF100E;

        public uint BufHeightAddr { get; set; } = 0xFFFF100F;

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
                   addr == BufWidthAddr;
        }

        #endregion

        #region Private

        static ConsoleInterfaceSettings()
        {
            SettingsCategory coutCategory = Peripheral.s_PeripheralCategory.AddCategory( "console" );

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
