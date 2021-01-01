using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.HostFS
{
    public class HostFileSystemSettings
    {
        public string RootPath = AppDomain.CurrentDomain.BaseDirectory;
        public uint PinStatus=0xFFFF3000;
        public uint PinSize = 0xFFFF3001;
        public uint PinOpen = 0xFFFF3002;
        public uint PinClose = 0xFFFF3003;
        public uint PinPath = 0xFFFF3004;
        public uint PinReset = 0xFFFF3005;
        public uint PinData = 0xFFFF3006;

        [XmlIgnore, JsonIgnore]
        public uint[] Pins => new[] { PinStatus, PinSize, PinOpen, PinClose, PinReset, PinPath, PinData };


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

    public class HostFileSystem : Peripheral
    {
        private HostFileSystemSettings settings;
        private StringBuilder sbPath = new StringBuilder();

        public HostFileSystem(HostFileSystemSettings settings)
        {
            this.settings = settings;
        }
        public HostFileSystem() : this(HostFileSystemSettings.Create()) { }
        
        public override bool CanRead(uint address)
        {
            return settings.Pins.Any(x => x == address);
        }

        public override bool CanWrite(uint address)
        {
            return settings.Pins.Any(x => x == address);
        }

        public override uint ReadData(uint address)
        {
            if (address == settings.PinStatus)
            {
                return 1;
            }
            if (address == settings.PinSize)
            {
                FileInfo fi = new FileInfo(GetPath(sbPath.ToString()));
                if (!fi.Exists)
                    return 0;
                return (uint)fi.Length;
            }
            return 0;
        }


        private string GetPath(string p)
        {
            return Path.Combine(settings.RootPath, p);
        }
        public override void WriteData(uint address, uint data)
        {
            if (address == settings.PinReset)
            {
                sbPath.Clear();
            }
            else if (address == settings.PinPath)
            {
                sbPath.Append((char)data);
            }
        }
    }
}