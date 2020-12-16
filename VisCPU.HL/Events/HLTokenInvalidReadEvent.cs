using VisCPU.HL.Parser;
using VisCPU.Utility.Events;

namespace VisCPU.HL
{

    public class HLTokenInvalidReadEvent : ErrorEvent
    {

        private const string EVENT_KEY = "hl-parser-token-invalid";
        public HLTokenInvalidReadEvent(HLTokenType expected, HLTokenType got) : base($"Expected Token '{expected}' but got '{got}'", EVENT_KEY, false)
        {
        }

    }

}