using System;
using System.IO;
using System.Linq;
using System.Text;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.HostFS
{
    public class HostFileSystemSettings
    {
        public string RootPath;
        public uint PinStatus;
        public uint PinSize;
        public uint PinOpen;
        public uint PinClose;
        public uint PinReset;
        public uint PinPath;
        public uint PinData;

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