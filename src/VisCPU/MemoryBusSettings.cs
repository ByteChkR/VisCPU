﻿using System.IO;

using VisCPU.Peripherals;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU
{

    public class MemoryBusSettings
    {

        [field: Argument( Name = "memory-bus:devices.memory" )]
        public string[] MemoryDevices { get; set; } =
            {
                Path.Combine(
                             Peripheral.PeripheralCategory.GetCategory( "memory" ).GetCategoryDirectory(),
                             "default.json"
                            )
            };

        #region Private

        static MemoryBusSettings()
        {
            SettingsCategory busCategory = CpuSettings.CpuCategory;

            SettingsManager.RegisterDefaultLoader(
                                                  new JsonSettingsLoader(),
                                                  busCategory,
                                                  "memory-bus.json",
                                                  new MemoryBusSettings()
                                                 );
        }

        #endregion

    }

}
