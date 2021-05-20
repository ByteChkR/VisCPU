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
        public uint InterfacePresentPin = 0xFFFF1010;

        [field: Argument( Name = "console:in.pin.read" )]
        public uint ReadInputAddress = 0xFFFF1011;

        [field: Argument( Name = "console:in.pin.available" )]
        public uint InputAvailableAddress = 0xFFFF1012;

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
