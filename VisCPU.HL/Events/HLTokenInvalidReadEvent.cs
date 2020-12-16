using VisCPU.HL.Parser;
using VisCPU.Utility.Events;

namespace VisCPU.HL.Events
{

    public class HLTokenInvalidReadEvent : ErrorEvent
    {

        private const string EVENT_KEY = "hl-parser-token-invalid";

        #region Public

        public HLTokenInvalidReadEvent( HLTokenType expected, HLTokenType got ) : base(
             $"Expected Token '{expected}' but got '{got}'",
             EVENT_KEY,
             false
            )
        {
        }

        #endregion

    }

}
