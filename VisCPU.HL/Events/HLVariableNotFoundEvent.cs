using VisCPU.Utility.Events;

namespace VisCPU.HL
{

    public class HLVariableNotFoundEvent : ErrorEvent
    {

        private const string EVENT_KEY = "hl-var-not-found";
        public HLVariableNotFoundEvent(string varName, bool canContinue) : base($"Can not find variable: {varName}", EVENT_KEY, canContinue)
        {
        }

    }

}