using System;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Peripherals.Console.IO
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
            SettingsCategory cinCategory = Peripheral.PeripheralCategory.AddCategory( "console" );

            SettingsManager.RegisterDefaultLoader(
                                                  new JsonSettingsLoader(),
                                                  cinCategory,
                                                  "in.json",
                                                  new ConsoleInInterfaceSettings()
                                                 );
        }

        #endregion

    }

}
