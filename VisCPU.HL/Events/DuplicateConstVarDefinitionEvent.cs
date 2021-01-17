using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

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
