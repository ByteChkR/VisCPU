using VisCPU.Utility.Settings;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU.Peripherals.Console
{

    public class ConsoleInterfaceSettings
    {
        public uint InterfacePresentPin = 0xFFFF1006;
        public uint WidthAddr = 0xFFFF1007;
        public uint HeightAddr = 0xFFFF1008;

        public uint CursorLeftAddr = 0xFFFF1009;
        public uint CursorTopAddr = 0xFFFF100A;

        public uint BackColorAddr = 0xFFFF100B;
        public uint ForeColorAddr = 0xFFFF100C;
        public uint ResetColorAddr = 0xFFFF100D;

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
                   addr == ResetColorAddr;
        }

        #endregion


        static ConsoleInterfaceSettings()
        {
            SettingsCategory coutCategory = Peripheral.s_PeripheralCategory.AddCategory("console");

            SettingsManager.RegisterDefaultLoader(
                new JsonSettingsLoader(),
                coutCategory,
                "management.json",
                new ConsoleInterfaceSettings()
            );
        }
    }

}