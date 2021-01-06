namespace VisCPU.Peripherals.HostFS
{

    public enum HostFileSystemCommands : uint
    {

        HFS_DEVICE_RESET = 0x00,
        HFS_OPEN_READ = 0x01,
        HFS_OPEN_WRITE = 0x02,
        HFS_CLOSE = 0x03,
        HFS_FILE_SIZE = 0x04,
        HFS_CHANGE_DIR = 0x05,
        HFS_CURRENT_DIR = 0x06,
        HFS_MAKE_DIR = 0x07,
        HFS_FILE_DELETE = 0x08,
        HFS_GET_FILES_NUM = 0x09,
        HFS_GET_FILES = 0x0A,


    }

}
