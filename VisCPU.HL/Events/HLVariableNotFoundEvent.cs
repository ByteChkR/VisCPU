using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Events
{

    public class HLVariableNotFoundEvent : ErrorEvent
    {

        #region Public

        public HLVariableNotFoundEvent( string varName, bool canContinue ) : base(
             $"Can not find variable: {varName}",
             ErrorEventKeys.HL_VARIABLE_NOT_FOUND,
             canContinue
            )
        {
        }

        #endregion

    }

}
