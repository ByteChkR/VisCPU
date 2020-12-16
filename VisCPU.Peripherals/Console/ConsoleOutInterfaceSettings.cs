﻿using System;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.Console
{

    [Serializable]
    public class ConsoleOutInterfaceSettings
    {

        [Argument( Name = "console:out.pin.read" )]
        public uint InterfacePresentPin = 0xFFFF1000;

        [Argument( Name = "console:out.pin.write.char" )]
        public uint WriteOutputAddress = 0xFFFF1001;

        [Argument( Name = "console:out.pin.write.num" )]
        public uint WriteNumOutputAddress = 0xFFFF1002;

        #region Public

        public static ConsoleOutInterfaceSettings Create()
        {
            return Settings.GetSettings < ConsoleOutInterfaceSettings >();
        }

        #endregion

        #region Private

        static ConsoleOutInterfaceSettings()
        {
            Settings.RegisterDefaultLoader(
                                           new JSONSettingsLoader(),
                                           "./config/console/out.json",
                                           new ConsoleOutInterfaceSettings()
                                          );
        }

        #endregion

    }

}
