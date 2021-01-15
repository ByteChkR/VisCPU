using System;
using System.IO;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.Console
{

    [Serializable]
    public class ConsoleOutInterfaceSettings
    {

        [field: Argument( Name = "console:out.pin.read" )]
        public uint InterfacePresentPin { get; } = 0xFFFF1000;

        [field: Argument( Name = "console:out.pin.write.num" )]
        public uint WriteNumOutputAddress { get; } = 0xFFFF1002;

        [field: Argument( Name = "console:out.pin.write.char" )]
        public uint WriteOutputAddress { get; } = 0xFFFF1001;

        [field: Argument( Name = "console:out.pin.clear" )]
        public uint InterfaceClearPin { get; } = 0xFFFF1005;

        #region Public

        public static ConsoleOutInterfaceSettings Create()
        {
            return SettingsSystem.GetSettings < ConsoleOutInterfaceSettings >();
        }

        #endregion

        #region Private

        static ConsoleOutInterfaceSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                                                 new JSONSettingsLoader(),
                                                 Path.Combine(
                                                              AppDomain.CurrentDomain.BaseDirectory,
                                                              "./config/console/out.json"
                                                             ),
                                                 new ConsoleOutInterfaceSettings()
                                                );
        }

        #endregion

    }

}
