using System;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU.Peripherals.Console
{

    [Serializable]
    public class ConsoleInInterfaceSettings
    {
        [field: Argument( Name = "console:in.pin.present" )]
        public uint InterfacePresentPin { get; set; } = 0xFFFF1003;

        [field: Argument( Name = "console:in.pin.read" )]
        public uint ReadInputAddress { get; set; } = 0xFFFF1004;

        #region Private

        static ConsoleInInterfaceSettings()
        {
            SettingsCategory cinCategory = Peripheral.s_PeripheralCategory.AddCategory( "console" );

            SettingsManager.RegisterDefaultLoader(
                new JSONSettingsLoader(),
                cinCategory,
                "in.json",
                new ConsoleInInterfaceSettings()
            );
        }

        #endregion
    }

}
