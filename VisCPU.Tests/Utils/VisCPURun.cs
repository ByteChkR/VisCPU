using System.IO;

using VisCPU.Peripherals;
using VisCPU.Utility;

namespace VisCPU.Tests
{

    public static class VisCPURun
    {
        

        public static void Run( string file, TestDevice testDevice )
        {
            MemoryBus bus = new MemoryBus(new Memory(0xFFFF + 1, 0), testDevice
                                         );
            CPU cpu = new CPU(bus, 0x00, 0x00);
            cpu.LoadBinary(File.ReadAllBytes(file).ToUInt());
            cpu.Run();
        }

    }

}