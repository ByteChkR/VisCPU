using VisCPU.Utility.Events;

namespace VisCPU
{

    public class DuplicateVarDefinitionEvent : ErrorEvent
    {

        private const string EVENT_KEY = "var-duplicate-def";
        public DuplicateVarDefinitionEvent(string varName) : base($"Duplicate Definition of: {varName}", EVENT_KEY, false)
        {
        }

    }

}