using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Events
{

    internal class DuplicateConstVarDefinitionEvent : ErrorEvent
    {

        #region Public

        public DuplicateConstVarDefinitionEvent( string varName ) : base(
                                                                         $"Duplicate Definition of: {varName}",
                                                                         ErrorEventKeys.s_HlConstVarDuplicateDef,
                                                                         false
                                                                        )
        {
        }

        #endregion

    }

}
