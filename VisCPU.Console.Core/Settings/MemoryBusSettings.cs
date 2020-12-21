﻿using System.Linq;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Settings
{
    public class MemoryBusSettings
    {
        [Argument(Name = "bus.devices")]
        public string[] MemoryDevices = new[] { "./config/memory/default.json" };

        static MemoryBusSettings()
        {
            Utility.Settings.Settings.RegisterDefaultLoader(new JSONSettingsLoader(), "./config/memory_bus.json", new MemoryBusSettings());
        }

        public static MemoryBusSettings Create() => Utility.Settings.Settings.GetSettings<MemoryBusSettings>();

        public MemoryBus CreateBus(params Peripheral[] additionalPeripherals)
        {
            return new MemoryBus(
                MemoryDevices.Select(
                    x => new Memory(
                        Utility.Settings.Settings.GetSettings < MemorySettings >(
                            x
                        )
                    )
                ).Concat(additionalPeripherals)
            );
        }

    }
}