using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Events
{

    internal class DuplicateVarDefinitionEvent : ErrorEvent
    {
        #region Public

        public DuplicateVarDefinitionEvent( string varName ) : base(
            $"Duplicate Definition of: {varName}",
            ErrorEventKeys.s_HlVarDuplicateDef,
            false
        )
        {
        }

        #endregion
    }

}
