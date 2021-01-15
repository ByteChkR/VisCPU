using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Assembler.Events
{

    public class TokenRecognitionFailureEvent : ErrorEvent
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
