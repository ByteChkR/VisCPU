using System;

namespace VisCPU.Peripherals.Drive
{

    [Serializable]
    public class DrivePinSettings
    {

        public uint PresentAddress = 0xFFFF6000;
        public uint GetSizeAddress = 0xFFFF6001;
        public uint WriteAddress = 0xFFFF6002;
        public uint WriteBufferAddress = 0xFFFF6003;
        public uint ReadAddress = 0xFFFF6004;
        public uint ReadBufferAddress = 0xFFFF6005;

    }

}
