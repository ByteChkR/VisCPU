using System.IO;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility;

namespace VisCPU.Tests.Utils
{

    public static class VisCpuRun
    {
        #region Public

        public static void Run( string file, TestDevice testDevice )
        {
            MemoryBus bus = new MemoryBus(
                new Memory(),
                testDevice
            );

            Cpu cpu = new Cpu( bus, 0x00, 0x00 );
            cpu.LoadBinary( File.ReadAllBytes( file ).ToUInt() );
            cpu.Run();
        }

        #endregion
    }

}
