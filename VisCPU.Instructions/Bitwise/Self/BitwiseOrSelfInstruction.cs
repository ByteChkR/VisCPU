using System;

namespace VisCPU.Instructions.Bitwise.Self
{

    public class BitwiseOrSelfInstruction : BitwiseInstruction
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 2;

        public override string Key => "OR";

        #region Public

        public override void Process( CPU cpu )
        {
            uint addressA = cpu.DecodeArgument( 0 ); //Number A Address
            uint addressB = cpu.DecodeArgument( 1 ); //Number B Address

            uint a = cpu.MemoryBus.Read( addressA ); //Read Value From RAM
            uint b = cpu.MemoryBus.Read( addressB ); //Read Value From RAM

            uint result = a | b; //Calculate Value

            Log(
                cpu,
                $"0x{Convert.ToString( addressA, 16 )}({Convert.ToString( a, 16 )}) | 0x{Convert.ToString( addressB, 16 )}({Convert.ToString( b, 16 )}) = 0x{Convert.ToString( addressA, 16 )}({Convert.ToString( result, 16 )})"
               );

            cpu.MemoryBus.Write( addressA, result ); //Write back Result
        }

        #endregion

    }

}
