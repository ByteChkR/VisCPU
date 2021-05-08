using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Events
{

    public class TypeNotFoundEvent : ErrorEvent
    {
        #region Public

        public TypeNotFoundEvent( string typeName ) : base(
            $"Can not find type with name {typeName}",
            ErrorEventKeys.s_HlTypeNotFound,
            false
        )
        {
        }

        #endregion
    }

}
