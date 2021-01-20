using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Events
{

    internal class HlVariableNotFoundEvent : ErrorEvent
    {
        #region Public

        public HlVariableNotFoundEvent( string varName, bool canContinue ) : base(
            $"Can not find variable: {varName}",
            ErrorEventKeys.s_HlVariableNotFound,
            canContinue
        )
        {
        }

        #endregion
    }

}
