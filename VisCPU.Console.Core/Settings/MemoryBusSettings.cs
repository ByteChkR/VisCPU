using System;
using System.IO;
using System.Linq;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Settings
{
    public class MemoryBusSettings
    {
        [Argument(Name = "memory-bus:devices")]
        public string[] MemoryDevices =
            {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./config/memory/default.json")};

        static MemoryBusSettings()
        {
            SettingsSystem.RegisterDefaultLoader(new JSONSettingsLoader(),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./config/memory_bus.json"),
                new MemoryBusSettings());
        }

        public static MemoryBusSettings Create()
        {
            return SettingsSystem.GetSettings<MemoryBusSettings>();
        }

        public MemoryBus CreateBus(params Peripheral[] additionalPeripherals)
        {
            return new MemoryBus(
                MemoryDevices.Select(
                    x => new Memory(
                        SettingsSystem.GetSettings<MemorySettings>(
                            x
                        )
                    )
                ).Concat(additionalPeripherals)
            );
        }
    }
}