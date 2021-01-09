using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VisCPU.Compiler.Assembler.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Emit
{
    public abstract class Emitter<EmitType>: VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.Emit;



        public abstract EmitType Emit( string instructionKey, params string[] arguments );

    }

    public class VasmEmitter : Emitter < string >
    {

        public override string Emit( string instructionKey, params string[] arguments )
        {
            StringBuilder sb = new StringBuilder(instructionKey);
            Instruction instr = CPUSettings.InstructionSet.GetInstruction(
                                                                          instructionKey,
                                                                          arguments.Length
                                                                         );
            if (arguments.Length > instr.ArgumentCount)
            {
                EventManager<ErrorEvent>.SendEvent(new InvalidArgumentCountEvent(instructionKey, arguments.Length));
            }

            foreach (string aToken in arguments)
            {
                sb.Append( $" {aToken}" );
            }

            return sb.ToString();
        }

    }

    public class BinaryEmitter : Emitter < byte[] >
    {

        public override byte[] Emit( string instructionKey, params string[] arguments )
        {
            List<byte> bytes = new List<byte>();

            Instruction instr = CPUSettings.InstructionSet.GetInstruction(
                                                                          instructionKey,
                                                                          arguments.Length
                                                                         );
            if (arguments.Length > instr.ArgumentCount)
            {
                EventManager<ErrorEvent>.SendEvent(new InvalidArgumentCountEvent(instructionKey, arguments.Length));
            }

            uint opCode =
                CPUSettings.InstructionSet.GetInstruction(
                                                          instr
                                                         );

            bytes.AddRange(BitConverter.GetBytes(opCode));

            foreach (string aToken in arguments)
            {
                bytes.AddRange(BitConverter.GetBytes(uint.Parse(aToken)));
            }
            if (bytes.Count > CPUSettings.BYTE_SIZE)
            {
                EventManager<ErrorEvent>.SendEvent(new InvalidInstructionArgumentCountEvent(instructionKey, arguments.Length));
            }

            bytes.AddRange(Enumerable.Repeat((byte)0, CPUSettings.BYTE_SIZE - bytes.Count));

            return bytes.ToArray();
        }

    }

}
