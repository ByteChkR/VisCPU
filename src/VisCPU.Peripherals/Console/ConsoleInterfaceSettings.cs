using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Peripherals.Console
{

    public class ConsoleInterfaceSettings
    {

        public uint InterfacePresentPin  = 0xFFFF1020;

        public uint InterfaceClearPin  = 0xFFFF1021;

        public uint WidthAddr  = 0xFFFF1022;

        public uint HeightAddr  = 0xFFFF1023;

        public uint CursorLeftAddr  = 0xFFFF1024;

        public uint CursorTopAddr  = 0xFFFF1025;

        public uint BackColorAddr  = 0xFFFF1026;

        public uint ForeColorAddr  = 0xFFFF1027;

        public uint ResetColorAddr  = 0xFFFF1028;

        public uint BufWidthAddr  = 0xFFFF1029;

        public uint BufHeightAddr  = 0xFFFF102A;

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
