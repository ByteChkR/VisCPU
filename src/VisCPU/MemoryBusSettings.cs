using System.IO;
using VisCPU.Peripherals;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU
{

    public class MemoryBusSettings
    {
        [field: Argument( Name = "memory-bus:devices" )]
        public string[] MemoryDevices { get; set; } =
        {
            Path.Combine(
                Peripheral.s_PeripheralCategory.GetCategory( "memory" ).GetCategoryDirectory(),
                "default.json"
            )
        };

        #region Private

        static MemoryBusSettings()
        {
            SettingsCategory busCategory = CpuSettings.s_CpuCategory;

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
