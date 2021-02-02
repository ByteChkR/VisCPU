using VisCPU.HL.Parser;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

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
