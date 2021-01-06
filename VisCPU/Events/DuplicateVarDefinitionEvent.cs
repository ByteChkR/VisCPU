using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Events
{

    public class DuplicateVarDefinitionEvent : ErrorEvent
    {

        #region Public

        public DuplicateVarDefinitionEvent( string varName ) : base(
                                                                    $"Duplicate Definition of: {varName}",
                                                                    ErrorEventKeys.HL_VAR_DUPLICATE_DEF,
                                                                    false
                                                                   )
        {
        }

        #endregion

    }

}
