using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Math.Float
{

    public class InvertFSelfInstruction : Instruction
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        public override string Key => "INV.F";

        protected override LoggerSystems SubSystem => LoggerSystems.MathInstructions;

        #region Public

        public override void Process( Cpu cpu )
        {
            uint addressA = cpu.DecodeArgument( 0 ); //Number A Address

            uint a = cpu.MemoryBus.Read( addressA ); //Read Value From RAM

            uint result = a ^ 0x80000000; //Access Bit Sign Directly (interpret UINT as float)

            cpu.MemoryBus.Write( addressA, result ); //Write back Result
        }

        #endregion

    }

}
