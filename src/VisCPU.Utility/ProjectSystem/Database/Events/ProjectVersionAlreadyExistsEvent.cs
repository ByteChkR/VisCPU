using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Utility.ProjectSystem.Database.Events
{

    internal class ProjectVersionAlreadyExistsEvent : WarningEvent
    {

        #region Public

        public ProjectVersionAlreadyExistsEvent( string moduleName, string version ) : base(
             $"Module {moduleName} already has a build published for version {version}",
             WarningEventKeys.s_ModuleVersionExists
            )
        {
        }

        #endregion

    }

}
