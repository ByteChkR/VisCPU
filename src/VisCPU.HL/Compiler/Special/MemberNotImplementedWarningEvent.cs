using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Special
{

    public class MemberNotImplementedWarningEvent : WarningEvent
    {
        #region Public

        public MemberNotImplementedWarningEvent( HlTypeDefinition type, HlMemberDefinition member ) : base(
            $"Ignoring unimplemented member '{member.Name}' in abstract type '{type.Namespace.FullName}::{type.Name}'",
            WarningEventKeys.s_HlMemberNotImplemented
        )
        {
        }

        #endregion
    }

}