using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Events
{

    internal class HLMemberRedefinitionEvent : ErrorEvent
    {
        #region Public

        public HLMemberRedefinitionEvent( string memberName, string typeName ) : base(
            $"Duplicate definition of {memberName} in type {typeName}",
            ErrorEventKeys.s_HlMemberDuplicateDef,
            true
        )
        {
        }

        #endregion
    }

}
