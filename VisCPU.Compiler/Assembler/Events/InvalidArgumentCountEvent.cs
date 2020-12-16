using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Assembler
{

    public class InvalidArgumentCountEvent : ErrorEvent
    {

        private const string EVENT_KEY = "asm-gen-too-many-args";
        public InvalidArgumentCountEvent(int line) : base($"Too many arguments in line: '{line}'", EVENT_KEY, false)
        {
        }

    }

}