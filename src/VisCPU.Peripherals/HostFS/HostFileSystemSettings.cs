using VisCPU.Utility;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU.Peripherals.HostFS
{

    public class HostFileSystemSettings
    {
        [field: Argument( Name = "hostfs:root.use" )]
        public bool UseRootPath { get; set; }

        [field: Argument( Name = "hostfs:delete.enable" )]
        public bool EnableDeleteFiles { get; set; }

        [field: Argument( Name = "hostfs:root" )]
        public string RootPath { get; set; } = AppRootHelper.AppRoot;

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
            SettingsCategory hfsCategory = Peripheral.s_PeripheralCategory.AddCategory( "host-fs" );

            SettingsManager.RegisterDefaultLoader(
                new JsonSettingsLoader(),
                hfsCategory,
                "default.json",
                new HostFileSystemSettings()
            );
        }

        #endregion
    }

}
