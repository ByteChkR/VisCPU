using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Compiler.Assembler.Events
{

    internal class TokenRecognitionFailureEvent : ErrorEvent
    {

        #region Public

        public TokenRecognitionFailureEvent( string value ) : base(
                                                                   $"Failed to recognize token '{value}'",
                                                                   ErrorEventKeys.s_AsmGenTokenRecognitionFailure,
                                                                   false
                                                                  )
        {
        }

        #endregion

    }

}
