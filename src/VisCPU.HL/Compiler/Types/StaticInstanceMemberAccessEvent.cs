using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Types
{

    public class StaticInstanceMemberAccessEvent : WarningEvent
    {
        #region Public

        public StaticInstanceMemberAccessEvent( HlTypeDefinition type, HlMemberDefinition member ) : base(
            $"Accessing Instance Function '{member.Name}' in type '{type.Namespace.FullName}::{type.Name}' as static function. Passing an instance of '{type.Namespace.FullName}::{type.Name}' is required",
            WarningEventKeys.s_StaticInstanceMemberAccess )
        {
        }

        #endregion
    }

}