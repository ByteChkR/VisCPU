using System.IO;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Peripherals.Memory
{

    public class MemorySettings
    {

        [field: Argument( Name = "memory:read" )]
        public bool EnableRead { get; set; } = true;

        [field: Argument( Name = "memory:write" )]
        public bool EnableWrite { get; set; } = true;

        [field: Argument( Name = "memory:persistent" )]
        public bool Persistent { get; set; }

        [field: Argument( Name = "memory:persistent.path" )]
        public string PersistentPath { get; set; } = Path.Combine(
                                                                  Peripheral.s_PeripheralCategory.
                                                                             AddCategory( "memory" ).
                                                                             AddCategory( "states" ).
                                                                             GetCategoryDirectory(),
                                                                  "default.bin"
                                                                 );

        [field: Argument( Name = "memory:size" )]
        public uint Size { get; set; } = 262144;

        #region Unity Event Functions

        [field: Argument( Name = "memory:start" )]
        public uint Start { get; set; }

        #endregion

        #region Private

        static MemorySettings()
        {
            SettingsCategory memoryCategory = Peripheral.s_PeripheralCategory.AddCategory( "memory" );

            SettingsManager.RegisterDefaultLoader(
                                                  new JsonSettingsLoader(),
                                                  memoryCategory,
                                                  "default.json",
                                                  new MemorySettings()
                                                 );
        }

        #endregion

    }

}
