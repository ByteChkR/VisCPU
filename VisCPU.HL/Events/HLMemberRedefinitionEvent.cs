using VisCPU.Utility.Events;

namespace VisCPU.HL.Events
{
    public class HLMemberRedefinitionEvent : ErrorEvent
    {
        private const string EVENT_KEY = "hl-member-redefinition";

        public HLMemberRedefinitionEvent(string memberName, string typeName) : base(
            $"Duplicate definition of {memberName} in type {typeName}", EVENT_KEY, true)
        {
        }
    }
}