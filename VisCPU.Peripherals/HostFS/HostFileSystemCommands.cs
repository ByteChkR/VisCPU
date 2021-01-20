namespace VisCPU.Peripherals.HostFS
{

    public enum HostFileSystemCommands : uint
    {
        HfsDeviceReset = 0x00,
        HfsOpenRead = 0x01,
        HfsOpenWrite = 0x02,
        HfsClose = 0x03,
        HfsFileSize = 0x04,
        HfsChangeDir = 0x05,
        HfsCurrentDir = 0x06,
        HfsMakeDir = 0x07,
        HfsFileDelete = 0x08,
        HfsGetFilesNum = 0x09,
        HfsGetFiles = 0x0A,
        HfsFileExist = 0x0B,
    }

}
