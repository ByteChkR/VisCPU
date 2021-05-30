using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Peripherals.Console
{

    public class ConsolePeripheralSettings
    {

        public uint InterfacePresentPin = 0xFFFF1000;


        //Output
        public uint WriteOutputAddress = 0xFFFF1001;
        public uint WriteNumOutputAddress = 0xFFFF1002;
        public uint WriteDirectAddress = 0xFFFF1003;

        //Input
        public uint ReadInputAddress = 0xFFFF1011;
        public uint InputAvailableAddress = 0xFFFF1012;

        //Management
        public uint InterfaceClearPin = 0xFFFF1021;
        public uint WidthAddr = 0xFFFF1022;
        public uint HeightAddr = 0xFFFF1023;
        public uint CursorLeftAddr = 0xFFFF1024;
        public uint CursorTopAddr = 0xFFFF1025;
        public uint BackColorAddr = 0xFFFF1026;
        public uint ForeColorAddr = 0xFFFF1027;
        public uint ResetColorAddr = 0xFFFF1028;
        public uint BufWidthAddr = 0xFFFF1029;
        public uint BufHeightAddr = 0xFFFF102A;

        #region Private

        static ConsolePeripheralSettings()
        {
            SettingsCategory coutCategory = Peripheral.PeripheralCategory.AddCategory("console");

            SettingsManager.RegisterDefaultLoader(
                                                  new JsonSettingsLoader(),
                                                  coutCategory,
                                                  "console.json",
                                                  new ConsolePeripheralSettings()
                                                 );
        }

        #endregion

    }

}