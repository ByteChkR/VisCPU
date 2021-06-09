using System;

using VisCPU.Peripherals;

namespace VisCPU.Integration
{

    public class ApiImporterDevice : Peripheral
    {

        private readonly string m_Name;
        private readonly uint m_ListenAddr;
        private readonly Func < Cpu, uint > m_InvokeExec;
        

        public override string PeripheralName => m_Name;

        public override PeripheralType PeripheralType => PeripheralType.Custom;

        public override uint PresentPin => m_ListenAddr;

        public override uint AddressRangeStart => m_ListenAddr;
        public override uint AddressRangeEnd => m_ListenAddr;

        #region Unity Event Functions

        public override void Reset()
        {
        }

        #endregion

        #region Public

        public ApiImporterDevice(string apiName, uint addr, Func < Cpu, uint > invokeExec )
        {
            m_Name = apiName ?? "_API_IMPORTER_"+addr;
            m_ListenAddr = addr;
            m_InvokeExec = invokeExec;
        }
        

        public override uint ReadData( uint address )
        {
            AttachedCpu.Push( m_InvokeExec( AttachedCpu ) );

            return CpuSettings.InstructionSet.GetInstruction( CpuSettings.InstructionSet.GetInstruction( "RET", 0 ) );
        }

        public override void WriteData( uint address, uint data )
        {
        }

        #endregion

    }

}
