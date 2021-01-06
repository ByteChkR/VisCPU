using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Events
{

    public class HLMemberRedefinitionEvent : ErrorEvent
    {

        #region Public

        public HLMemberRedefinitionEvent( string memberName, string typeName ) : base(
             $"Duplicate definition of {memberName} in type {typeName}",
             ErrorEventKeys.HL_MEMBER_DUPLICATE_DEF,
             true
            )
        {
        }

        #endregion

    }

}
