using System;
using System.IO;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.HostFS
{

    public class HostFileSystemSettings
    {

        [field: Argument( Name = "hostfs:root.use" )]
        public bool UseRootPath { get; set; }

        [field: Argument( Name = "hostfs:delete.enable" )]
        public bool EnableDeleteFiles { get; set; }

        [field: Argument( Name = "hostfs:root" )]
        public string RootPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;

        [field: Argument( Name = "hostfs:pin.present" )]
        public uint PinPresent { get; set; } = 0xFFFF3000;

        [field: Argument( Name = "hostfs:pin.status" )]
        public uint PinStatus { get; set; } = 0xFFFF3001;

        [field: Argument( Name = "hostfs:pin.data" )]
        public uint PinData { get; set; } = 0xFFFF3002;

        [field: Argument( Name = "hostfs:pin.cmd" )]
        public uint PinCmd { get; set; } = 0xFFFF3003;
        

        #region Private

        static HostFileSystemSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                                                 new JSONSettingsLoader(),
                                                 Path.Combine(
                                                              AppDomain.CurrentDomain.BaseDirectory,
                                                              "./config/host_fs/settings.json"
                                                             ),
                                                 new HostFileSystemSettings()
                                                );
        }

        #endregion

    }

}
