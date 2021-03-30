using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Utility.ProjectSystem.Database.Events
{

    internal class ProjectDatabaseUnsupportedEvent : ErrorEvent
    {
        #region Public

        public ProjectDatabaseUnsupportedEvent( ProjectDatabase manager, string feature ) : base(
            $"'{manager}' does not support {feature}",
            ErrorEventKeys.s_ModuleManagerUnsupportedFeature,
            false
        )
        {
        }

        #endregion
    }

}
