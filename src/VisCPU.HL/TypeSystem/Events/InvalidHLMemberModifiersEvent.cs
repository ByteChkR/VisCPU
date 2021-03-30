using VisCPU.HL.Parser;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.TypeSystem.Events
{

    internal abstract class InvalidHlMemberModifiersEvent : ErrorEvent
    {
        #region Protected

        protected InvalidHlMemberModifiersEvent( HlTokenType a, HlTokenType b ) : base(
            $"Token '{a}' can not be used together with '{b}'",
            ErrorEventKeys.s_HlInvalidMemberModifiers,
            false
        )
        {
        }

        #endregion
    }

}
