using System.IO;

using VisCPU.Peripherals.Memory;
using VisCPU.Utility;
using VisCPU.Utility.Settings;

namespace VisCPU.Tests.Utils
{

    public static class VisCPURun
    {

        #region Public

        public static void Run( string file, TestDevice testDevice )
        {
            
            MemoryBus bus = new MemoryBus(
                                          new Memory(),
                                          testDevice
                                         );

            CPU cpu = new CPU( bus, 0x00, 0x00 );
            cpu.LoadBinary( File.ReadAllBytes( file ).ToUInt() );
            cpu.Run();
        }

        #endregion

    }

}
