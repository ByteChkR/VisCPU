using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Console.Core.Subsystems.Project.Events
{

    internal class ProjectFileNotFoundEvent : ErrorEvent
    {

        #region Public

        public ProjectFileNotFoundEvent( string projectRoot ) : base(
                                                                     $"The folder '{projectRoot}' does not contain a 'project.json' file.",
                                                                     ErrorEventKeys.s_ModuleFileNotFound,
                                                                     false
                                                                    )
        {
        }

        #endregion

    }

}
