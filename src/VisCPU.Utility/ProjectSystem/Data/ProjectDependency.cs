using System;

namespace VisCPU.Utility.ProjectSystem.Data
{

    [Serializable]
    public class ProjectDependency
    {
        public string ProjectName { get; set; }

        public string ProjectVersion { get; set; }
    }

}
