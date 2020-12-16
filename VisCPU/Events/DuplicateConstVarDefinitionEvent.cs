using VisCPU.Utility.Events;

namespace VisCPU
{

    public class DuplicateConstVarDefinitionEvent : ErrorEvent
    {

        private const string EVENT_KEY = "var-duplicate-def";
        public DuplicateConstVarDefinitionEvent(string varName) : base($"Duplicate Definition of: {varName}", EVENT_KEY, false)
        {
        }

    }

}