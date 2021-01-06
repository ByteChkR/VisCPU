namespace VisCPU.Peripherals.HostFS
{

    public enum HostFileSystemCommands : uint
    {

        HFS_DEVICE_RESET = 0x00,
        HFS_OPEN_READ = 0x01,
        HFS_OPEN_WRITE = 0x02,
        HFS_CLOSE = 0x03,
        HFS_FILE_SIZE = 0x04,

    }

}
