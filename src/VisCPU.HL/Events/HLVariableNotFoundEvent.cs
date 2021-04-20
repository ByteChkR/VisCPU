using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

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
