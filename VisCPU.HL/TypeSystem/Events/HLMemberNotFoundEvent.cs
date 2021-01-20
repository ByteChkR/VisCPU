using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.TypeSystem.Events
{

    internal class HLMemberNotFoundEvent : ErrorEvent
    {
        #region Public

        public HLMemberNotFoundEvent( string name ) : base(
            $"Can not Find Member: {name}",
            ErrorEventKeys.s_HlMemberNotFound,
            false
        )
        {
        }

        #endregion
    }

}
