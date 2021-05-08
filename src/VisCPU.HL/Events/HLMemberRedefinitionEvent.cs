using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Events
{

    internal class HlMemberRedefinitionEvent : ErrorEvent
    {
        #region Public

        public HlMemberRedefinitionEvent( string memberName, string typeName ) : base(
            $"Duplicate definition of {memberName} in type {typeName}",
            ErrorEventKeys.s_HlMemberDuplicateDef,
            true
        )
        {
        }

        #endregion
    }

}
