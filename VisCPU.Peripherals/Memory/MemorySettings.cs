using System;
using System.IO;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.Memory
{
    public class MemorySettings
    {
        [Argument(Name = "memory:read")] public bool EnableRead = true;
        [Argument(Name = "memory:write")] public bool EnableWrite = true;
        [Argument(Name = "memory:persistent")] public bool Persistent = false;
        [Argument(Name = "memory:persistent.path")]
        public string PersistentPath = "./config/memory/states/default.bin";
        [Argument(Name = "memory:size")] public uint Size = 0xFFFF;
        [Argument(Name = "memory:start")] public uint Start = 0;

        static MemorySettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                new JSONSettingsLoader(),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "./config/memory/default.json"),
                new MemorySettings()
            );
        }

        public static MemorySettings Create()
        {
            return SettingsSystem.GetSettings<MemorySettings>();
        }
    }
}