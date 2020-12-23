using VisCPU.Utility.Events;

namespace VisCPU.HL.Events
{
    public class HLTypeRedefinitionEvent : ErrorEvent
    {
        private const string EVENT_KEY = "hl-type-redefinition";

        public HLTypeRedefinitionEvent(string typeName) : base($"Duplicate definition of type {typeName}", EVENT_KEY,
            false)
        {
        }
    }
}