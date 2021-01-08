using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Events
{

    public class DuplicateConstVarDefinitionEvent : ErrorEvent
    {

        #region Public

        public DuplicateConstVarDefinitionEvent( string varName ) : base(
                                                                         $"Duplicate Definition of: {varName}",
                                                                         ErrorEventKeys.HL_CONST_VAR_DUPLICATE_DEF,
                                                                         false
                                                                        )
        {
        }

        #endregion

    }

}
