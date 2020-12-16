using VisCPU.Utility.Events;

namespace VisCPU.Events
{

    public class DuplicateVarDefinitionEvent : ErrorEvent
    {

        private const string EVENT_KEY = "var-duplicate-def";

        #region Public

        public DuplicateVarDefinitionEvent( string varName ) : base(
                                                                    $"Duplicate Definition of: {varName}",
                                                                    EVENT_KEY,
                                                                    false
                                                                   )
        {
        }

        #endregion

    }

}
