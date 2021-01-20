using System;
using VisCPU.Peripherals;

namespace VisCPU.HL.Integration
{

    public class APIImporterDevice : Peripheral
    {
        private CPU ExecutingCPU;
        private readonly uint ListenAddr;
        private readonly Func < CPU, uint > InvokeExec;

        #region Public

        public APIImporterDevice( uint addr, Func < CPU, uint > invokeExec )
        {
            ListenAddr = addr;
            InvokeExec = invokeExec;
        }

        public override bool CanRead( uint address )
        {
            return address == ListenAddr;
        }

        public override bool CanWrite( uint address )
        {
            return address == ListenAddr;
        }

        public override uint ReadData( uint address )
        {
            ExecutingCPU.Push( InvokeExec( ExecutingCPU ) );

            return CPUSettings.InstructionSet.GetInstruction( CPUSettings.InstructionSet.GetInstruction( "RET", 0 ) );
        }

        public void SetExecutingCPU( CPU executingCPU )
        {
            ExecutingCPU = executingCPU;
        }

        public override void WriteData( uint address, uint data )
        {
        }

        #endregion
    }

}
