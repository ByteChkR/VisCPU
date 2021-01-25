namespace VisCPU.Instructions.Math
{

    public abstract class MathSelfInstruction : MathInstruction
    {
        #region Public

        public override void Process( Cpu cpu )
        {
            uint addressA = cpu.DecodeArgument( 0 ); //Number A Address
            uint addressB = cpu.DecodeArgument( 1 ); //Number B Address

            uint a = cpu.MemoryBus.Read( addressA ); //Read Value From RAM
            uint b = cpu.MemoryBus.Read( addressB ); //Read Value From RAM

            uint result = Calculate( a, b ); //Calculate Value

            cpu.MemoryBus.Write( addressA, result ); //Write back Result
        }

        #endregion
    }

}
