using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Parser.Tokens
{

    public class InvalidDecValueEvent : ErrorEvent
    {

        private const string EVENT_KEY = "invalid-dec-value";
        public InvalidDecValueEvent(string value, int start) : base($"Invalid decimal Value: '{value}' at line {start}", EVENT_KEY, false)
        {
        }

    }

}