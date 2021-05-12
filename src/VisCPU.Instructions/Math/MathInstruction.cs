using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Math
{

    public abstract class MathInstruction : Instruction
    {

        protected override LoggerSystems SubSystem => LoggerSystems.MathInstructions;

        #region Public

        public abstract uint Calculate( uint a, uint b );

        public override void Process( Cpu cpu )
        {
            uint addressA = cpu.DecodeArgument( 0 );      //Number A Address
            uint addressB = cpu.DecodeArgument( 1 );      //Number B Address
            uint addressResult = cpu.DecodeArgument( 2 ); //Result Address

            uint a = cpu.MemoryBus.Read( addressA ); //Read Value From RAM
            uint b = cpu.MemoryBus.Read( addressB ); //Read Value From RAM

            uint result = Calculate( a, b ); //Calculate Value

            cpu.MemoryBus.Write( addressResult, result ); //Write back Result
        }

        #endregion

    }

}
