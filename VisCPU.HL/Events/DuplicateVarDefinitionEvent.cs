using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

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
