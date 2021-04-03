using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Special
{

    public class MemberNotImplementedErrorEvent : ErrorEvent
    {
        #region Public

        public MemberNotImplementedErrorEvent( HlTypeDefinition type, HlMemberDefinition member ) : base(
            $"FunctionType '{type.Name}' does not implement member '{member.Name}'",
            ErrorEventKeys.s_HlMemberNotImplemented,
            false
        )
        {
        }

        #endregion
    }

}
