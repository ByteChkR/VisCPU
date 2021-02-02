using System;

namespace VisCPU.ProjectSystem.Data
{

    [Serializable]
    public class ProjectDependency
    {
        public string ProjectName { get; set; }

        public string ProjectVersion { get; set; }
    }

}
