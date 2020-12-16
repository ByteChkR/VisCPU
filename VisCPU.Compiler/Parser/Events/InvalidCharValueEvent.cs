using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Parser.Tokens
{

    public class InvalidCharValueEvent : ErrorEvent
    {

        private const string EVENT_KEY = "invalid-char-value";
        public InvalidCharValueEvent( string value, int start ) : base($"Invalid char Value: '{value}' at line {start}", EVENT_KEY, false )
        {
        }

    }

}