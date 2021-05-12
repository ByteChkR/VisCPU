using System;

using VisCPU.Peripherals;

namespace VisCPU.Integration
{

    public class ApiImporterDevice : Peripheral
    {

        private readonly uint m_ListenAddr;
        private readonly Func < Cpu, uint > m_InvokeExec;

        public override string PeripheralName => "__API_IMPORTER_" + PresentPin;

        public override PeripheralType PeripheralType => PeripheralType.Custom;

        public override uint PresentPin => m_ListenAddr;

        #region Unity Event Functions

        public override void Reset()
        {
        }

        #endregion

        #region Public

        public ApiImporterDevice( uint addr, Func < Cpu, uint > invokeExec )
        {
            m_ListenAddr = addr;
            m_InvokeExec = invokeExec;
        }

        public override bool CanRead( uint address )
        {
            return address == m_ListenAddr;
        }

        public override bool CanWrite( uint address )
        {
            return address == m_ListenAddr;
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
