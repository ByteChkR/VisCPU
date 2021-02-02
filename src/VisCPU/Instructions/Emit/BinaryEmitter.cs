using System;
using System.Collections.Generic;
using System.Linq;
using VisCPU.Instructions.Emit.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Instructions.Emit
{

    public class BinaryEmitter : Emitter < byte[] >
    {
        #region Public

        public override byte[] Emit( string instructionKey, params string[] arguments )
        {
            List < byte > bytes = new List < byte >();

            Instruction instr = CpuSettings.InstructionSet.GetInstruction(
                instructionKey,
                arguments.Length
            );

            if ( arguments.Length > instr.ArgumentCount )
            {
                EventManager < ErrorEvent >.SendEvent(
                    new InvalidArgumentCountEvent( instructionKey, arguments.Length )
                );
            }

            uint opCode =
                CpuSettings.InstructionSet.GetInstruction(
                    instr
                );

            bytes.AddRange( BitConverter.GetBytes( opCode ) );

            foreach ( string aToken in arguments )
            {
                bytes.AddRange( BitConverter.GetBytes( uint.Parse( aToken ) ) );
            }

            if ( bytes.Count > CpuSettings.ByteSize )
            {
                EventManager < ErrorEvent >.SendEvent(
                    new InvalidInstructionArgumentCountEvent(
                        instructionKey,
                        arguments.Length
                    )
                );
            }

            bytes.AddRange( Enumerable.Repeat( ( byte ) 0, ( int ) CpuSettings.ByteSize - bytes.Count ) );

            return bytes.ToArray();
        }

        #endregion
    }

}
