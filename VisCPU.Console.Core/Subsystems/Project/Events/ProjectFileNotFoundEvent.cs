using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Console.Core.Subsystems.Project
{

    public class ProjectFileNotFoundEvent:ErrorEvent
    {

        public ProjectFileNotFoundEvent( string projectRoot) : base($"The folder '{projectRoot}' does not contain a 'project.json' file.", ErrorEventKeys.s_ModuleFileNotFound, false )
        {
        }

    }

}
