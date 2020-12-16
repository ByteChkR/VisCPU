using VisCPU.Utility.Events;

namespace VisCPU.Events
{

    public class DuplicateConstVarDefinitionEvent : ErrorEvent
    {

        private const string EVENT_KEY = "var-duplicate-def";

        #region Public

        public DuplicateConstVarDefinitionEvent( string varName ) : base(
                                                                         $"Duplicate Definition of: {varName}",
                                                                         EVENT_KEY,
                                                                         false
                                                                        )
        {
        }

        #endregion

    }

}
