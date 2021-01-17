using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.ProjectSystem.Database.Events
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
