using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Linking
{

    public class DuplicateLinkerItemEvent : WarningEvent
    {

        private const string EVENT_KEY = "lnk-dup-item";
        public DuplicateLinkerItemEvent(string item) : base($"The item '{item}' has a duplicated entry.", EVENT_KEY)
        {
        }

    }

}