﻿using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Peripherals.Time
{

    public class TimeDeviceSettings
    {

        public uint PresentPin { get; set; } = 0xFFFF5000;

        public uint TimePin { get; set; } = 0xFFFF5001;

        #region Private

        static TimeDeviceSettings()
        {
            SettingsCategory timeCategory = Peripheral.PeripheralCategory.AddCategory( "time" );

            SettingsManager.RegisterDefaultLoader(
                                                  new JsonSettingsLoader(),
                                                  timeCategory,
                                                  "default.json",
                                                  new TimeDeviceSettings()
                                                 );
        }

        #endregion

    }

}
