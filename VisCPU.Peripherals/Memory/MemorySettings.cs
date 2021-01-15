using System;
using System.IO;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.Memory
{

    public class MemorySettings
    {

        [field: Argument( Name = "memory:read" )]
        public bool EnableRead { get; } = true;

        [field: Argument( Name = "memory:write" )]
        public bool EnableWrite { get; } = true;

        [field: Argument( Name = "memory:persistent" )]
        public bool Persistent { get; }

        [field: Argument( Name = "memory:persistent.path" )]
        public string PersistentPath { get; } = "./config/memory/states/default.bin";

        [field: Argument( Name = "memory:size" )]
        public uint Size { get; } = 262144;

        #region Unity Event Functions

        [field: Argument( Name = "memory:start" )]
        public uint Start { get; }

        #endregion

        #region Public

        public static MemorySettings Create()
        {
            return SettingsSystem.GetSettings < MemorySettings >();
        }

        #endregion

        #region Private

        static MemorySettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                                                 new JSONSettingsLoader(),
                                                 Path.Combine(
                                                              AppDomain.CurrentDomain.BaseDirectory,
                                                              "./config/memory/default.json"
                                                             ),
                                                 new MemorySettings()
                                                );
        }

        #endregion

    }

}
