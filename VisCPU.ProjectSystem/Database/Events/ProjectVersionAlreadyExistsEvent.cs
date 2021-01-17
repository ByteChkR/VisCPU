using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.ProjectSystem.Database.Events
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
