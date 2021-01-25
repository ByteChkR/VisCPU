using System;
using VisCPU.Peripherals;

namespace VisCPU.HL.Integration
{

    public class ApiImporterDevice : Peripheral
    {
        private Cpu m_ExecutingCpu;
        private readonly uint m_ListenAddr;
        private readonly Func < Cpu, uint > m_InvokeExec;

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
            m_ExecutingCpu.Push( m_InvokeExec( m_ExecutingCpu ) );

            return CpuSettings.InstructionSet.GetInstruction( CpuSettings.InstructionSet.GetInstruction( "RET", 0 ) );
        }

        public void SetExecutingCpu( Cpu executingCpu )
        {
            m_ExecutingCpu = executingCpu;
        }

        public override void WriteData( uint address, uint data )
        {
        }

        #endregion
    }

}
