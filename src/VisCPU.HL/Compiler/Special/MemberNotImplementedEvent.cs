using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Special
{

    public class MemberNotImplementedEvent : ErrorEvent
    {

        #region Public

        public MemberNotImplementedEvent( HlTypeDefinition type, HlMemberDefinition member ) : base(
             $"Type '{type.Name}' does not implement member '{member.Name}'",
             ErrorEventKeys.s_HlMemberNotImplemented,
             false
            )
        {
        }

        #endregion

    }

}
