namespace VisCPU.Peripherals.HostFS
{

    public enum HostFileSystemStatus : uint
    {
        HfsStatusOffline = 0x00,
        HfsStatusReady = 0x01,
        HfsStatusFileOpen = 0x02,
    }

}
