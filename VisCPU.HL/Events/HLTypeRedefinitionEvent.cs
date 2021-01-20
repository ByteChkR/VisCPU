using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Events
{

    internal class HlTypeRedefinitionEvent : ErrorEvent
    {
        #region Public

        public HlTypeRedefinitionEvent( string typeName ) : base(
            $"Duplicate definition of type {typeName}",
            ErrorEventKeys.s_HlTypeDuplicateDef,
            false
        )
        {
        }

        #endregion
    }

}
