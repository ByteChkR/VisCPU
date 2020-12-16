using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Assembler.Events
{

    public class TokenRecognitionFailureEvent : ErrorEvent
    {

        private const string EVENT_KEY = "asm-gen-token-recognition-failure";

        #region Public

        public TokenRecognitionFailureEvent( string value ) : base(
                                                                   $"Failed to recognize token '{value}'",
                                                                   EVENT_KEY,
                                                                   false
                                                                  )
        {
        }

        #endregion

    }

}
