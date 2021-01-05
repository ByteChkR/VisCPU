using System;
using System.IO;
using System.Linq;
using System.Text;

using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Peripherals.HostFS
{

    public class HostFileSystem : Peripheral
    {
        private HostFileSystemStatus status = HostFileSystemStatus.HFS_STATUS_READY;
        private HostFileSystemSettings settings;
        private StringBuilder sbPath = new StringBuilder();
        private FileInfo currentFile = null;
        private FileStream currentFileStream = null;
        private bool readFileSize = false;

        public HostFileSystem(HostFileSystemSettings settings)
        {
            this.settings = settings;
        }
        public HostFileSystem() : this(HostFileSystemSettings.Create()) { }

        public override bool CanRead(uint address)
        {

            return address == settings.PinData || address == settings.PinPresent || address == settings.PinStatus;
        }

        public override bool CanWrite(uint address)
        {
            return address == settings.PinCmd || address == settings.PinData;
        }

        public override uint ReadData(uint address)
        {
            if (address == settings.PinPresent)
            {
                return 1;
            }
            if (address == settings.PinStatus)
            {
                return (uint)status;
            }
            if (address == settings.PinData)
            {
                if (readFileSize)
                {
                    readFileSize = false;
                    return (uint)currentFile.Length/sizeof(uint);
                }
                if (currentFileStream.Length <= currentFileStream.Position)
                {
                    status = HostFileSystemStatus.HFS_STATUS_READY;
                    currentFileStream.Close();
                    currentFileStream = null;
                    currentFile = null;
                    return 0;
                }
                if (status == HostFileSystemStatus.HFS_STATUS_FILE_OPEN)
                {
                    byte[] buf = new byte[sizeof( uint )];
                    currentFileStream.Read( buf, 0, sizeof( uint ) );
                    return BitConverter.ToUInt32(buf, 0);
                }
            }
            return 0;
        }


        private string GetPath(string p)
        {
            if (settings.UseRootPath)
                return Path.Combine(settings.RootPath, p);
            return p;
        }

        public override void WriteData(uint address, uint data)
        {
            if (address == settings.PinData)
            {
                if (status == HostFileSystemStatus.HFS_STATUS_FILE_OPEN)
                {
                    currentFileStream.Write(BitConverter.GetBytes(data), 0, sizeof(uint));
                }
                else if (status == HostFileSystemStatus.HFS_STATUS_READY)
                {
                    char chr = (char)data;
                    sbPath.Append(chr);
                }
            }
            if (address == settings.PinCmd)
            {
                HostFileSystemCommands cmd = (HostFileSystemCommands)data;

                switch (cmd)
                {
                    case HostFileSystemCommands.HFS_DEVICE_RESET:
                        status = HostFileSystemStatus.HFS_STATUS_READY;
                        sbPath.Clear();
                        currentFileStream?.Close();
                        currentFileStream = null;
                        currentFile = null;
                        break;

                    case HostFileSystemCommands.HFS_OPEN_READ:
                        status = HostFileSystemStatus.HFS_STATUS_FILE_OPEN;
                        string pathR = GetPath(sbPath.ToString());
                        Log( "Opening File(READ): " + pathR );
                        if (!File.Exists(pathR))
                        {
                            EventManager<ErrorEvent>.SendEvent(new FileNotFoundEvent(pathR, false));
                        }
                        else
                        {
                            currentFileStream = File.OpenRead(pathR);
                            currentFile = new FileInfo(pathR);
                        }
                        sbPath.Clear();
                        break;

                    case HostFileSystemCommands.HFS_OPEN_WRITE:
                        status = HostFileSystemStatus.HFS_STATUS_FILE_OPEN;
                        string pathW = GetPath(sbPath.ToString());
                        Log("Opening File(WRITE): " + pathW);
                        if (!File.Exists(sbPath.ToString()))
                        {
                            currentFileStream = File.Create(pathW);
                            currentFile = new FileInfo(pathW);
                        }
                        else
                        {
                            currentFileStream = File.OpenWrite(pathW);
                            currentFile = new FileInfo(pathW);
                        }
                        sbPath.Clear();
                        break;

                    case HostFileSystemCommands.HFS_CLOSE:
                        Log("Closing File: " + currentFile.FullName);
                        status = HostFileSystemStatus.HFS_STATUS_READY;
                        sbPath.Clear();
                        currentFileStream?.Close();
                        currentFileStream = null;
                        currentFile = null;
                        break;

                    case HostFileSystemCommands.HFS_FILE_SIZE:
                        string p = GetPath(sbPath.ToString());
                        currentFile = new FileInfo(p);
                        sbPath.Clear();
                        readFileSize = true;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}