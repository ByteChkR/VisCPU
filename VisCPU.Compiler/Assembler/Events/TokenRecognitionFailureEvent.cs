using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Assembler
{

    public class TokenRecognitionFailureEvent : ErrorEvent
    {

        private const string EVENT_KEY = "asm-gen-token-recognition-failure";
        public TokenRecognitionFailureEvent(string value) : base($"Failed to recognize token '{value}'", EVENT_KEY, false)
        {
        }

    }

}