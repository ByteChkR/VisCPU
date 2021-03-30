using System;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Peripherals.Console.IO
{

    [Serializable]
    public class ConsoleOutInterfaceSettings
    {
        [field: Argument( Name = "console:out.pin.read" )]
        public uint InterfacePresentPin { get; set; } = 0xFFFF1000;

        [field: Argument( Name = "console:out.pin.write.num" )]
        public uint WriteNumOutputAddress { get; set; } = 0xFFFF1002;

        [field: Argument( Name = "console:out.pin.write.char" )]
        public uint WriteOutputAddress { get; set; } = 0xFFFF1001;

        [field: Argument( Name = "console:out.pin.clear" )]
        public uint InterfaceClearPin { get; set; } = 0xFFFF1005;

        #region Private

        static ConsoleOutInterfaceSettings()
        {
            SettingsCategory coutCategory = Peripheral.s_PeripheralCategory.AddCategory( "console" );

            SettingsManager.RegisterDefaultLoader(
                new JsonSettingsLoader(),
                coutCategory,
                "out.json",
                new ConsoleOutInterfaceSettings()
            );
        }

        #endregion
    }

}
