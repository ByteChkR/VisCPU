using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.HostFS
{
    public class HostFileSystemSettings
    {

        [Argument(Name = "hostfs:root.use")]
        public bool UseRootPath;
        [Argument(Name = "hostfs:root")]
        public string RootPath = AppDomain.CurrentDomain.BaseDirectory;
        [Argument(Name = "hostfs:pin.present")]
        public uint PinPresent = 0xFFFF3000;
        [Argument(Name = "hostfs:pin.status")]
        public uint PinStatus = 0xFFFF3001;
        [Argument(Name = "hostfs:pin.data")]
        public uint PinData = 0xFFFF3002;
        [Argument(Name = "hostfs:pin.cmd")]
        public uint PinCmd = 0xFFFF3003;

        #region Private
        static HostFileSystemSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                new JSONSettingsLoader(),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "./config/host_fs/settings.json"),
                new HostFileSystemSettings()
            );
        }

        #endregion

        #region Public

        public static HostFileSystemSettings Create()
        {
            return SettingsSystem.GetSettings<HostFileSystemSettings>();
        }
        #endregion
    }
}