using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using Newtonsoft.Json;

using VisCPU.HL.Modules.ModuleManagers;

namespace VisCPU.HL.Modules.Data
{

    [Serializable]
    public class ProjectInfo
    {

        [JsonIgnore]
        [XmlIgnore]
        public ModuleManager Manager;

        public string ProjectName = "MyProject";
        public string ProjectVersion = "0.0.0.1";
        public List < ProjectDependency > Dependencies = new List < ProjectDependency >();
        public ProjectInfo()
        {
        }
        public ProjectInfo(
            ModuleManager manager,
            string projectName,
            string projectVersion,
            ProjectDependency[] dependencies )
        {
            ProjectName = projectName;
            ProjectVersion = projectVersion;
            Manager = manager;
            Dependencies = dependencies.ToList();
        }

    }

}
