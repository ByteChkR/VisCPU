using VisCPU.Instructions.Math.Self.Float;

namespace VisCPU.Instructions.Math.Float
{

    public class DecFInstruction : SubFSelfInstruction
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        public override string Key => "DEC.F";

        #region Public

        public override void Process( Cpu cpu )
        {
            uint addressA = cpu.DecodeArgument( 0 ); //Number A Address

            uint a = cpu.MemoryBus.Read( addressA ); //Read Value From RAM

            uint result = ( this as MathInstruction ).Calculate( a, 1 ); //Calculate Value

            cpu.MemoryBus.Write( addressA, result ); //Write back Result
        }

        #endregion

    }

}
