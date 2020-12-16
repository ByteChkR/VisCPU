using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Compiler
{

    public class InvalidConstantDefinitionEvent : ErrorEvent
    {

        private const string EVENT_KEY = "invalid-const-def";
        public InvalidConstantDefinitionEvent(string message) : base(message, EVENT_KEY, false)
        {
        }

    }

}