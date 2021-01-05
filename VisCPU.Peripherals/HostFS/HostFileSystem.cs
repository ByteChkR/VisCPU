using System.IO;
using System.Linq;
using System.Text;

namespace VisCPU.Peripherals.HostFS
{
    public class HostFileSystem : Peripheral
    {
        private HostFileSystemSettings settings;
        private StringBuilder sbPath = new StringBuilder();
        private FileInfo currentFile = null;
        private FileStream currentFileStream = null;

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
            if (address == settings.PinData)
            {
                if (currentFile == null)
                {
                    currentFile = new FileInfo(GetPath(sbPath.ToString()));
                    currentFileStream = currentFile.OpenRead();
                }
                if (currentFileStream.Length <= currentFileStream.Position)
                {
                    currentFileStream.Close();
                    currentFileStream = null;
                    currentFile = null;
                    return 0;
                }
                return (uint) currentFileStream.ReadByte();

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
            else if (address == settings.PinPath && data != 0)
            {
                sbPath.Append((char)data);
            }
        }
    }
}