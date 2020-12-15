using System;

using VisCPU.Utility.Events;

namespace VisCPU
{
    public abstract class Instruction : VisBase
    {

        public abstract uint Cycles { get; }

        public abstract string Key { get; }

        public abstract uint InstructionSize { get; }

        public abstract uint ArgumentCount { get; }

        public abstract void Process(CPU cpu);

        protected void Log(CPU cpu, string mesg)
        {
            
                Log($"[0x{Convert.ToString(cpu.ProgramCounter, 16)}]: {mesg}");
            
        }

        public override void Log( string message )
        {
            base.Log( $"[{Key}] " + message );
        }

    }
}