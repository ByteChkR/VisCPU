using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VisCPU.HL.Modules.ModuleManagers;

namespace VisCPU.HL.Modules.Data
{
    [Serializable]
    public struct ModuleTarget
    {
        [JsonIgnore, XmlIgnore]
        public ModuleManager Manager;
        public string ModuleName;
        public string ModuleVersion;
        public List<ModuleDependency> Dependencies;

        public ModuleTarget(ModuleManager manager, string moduleName, string moduleVersion, ModuleDependency[] dependencies)
        {
            ModuleName = moduleName;
            ModuleVersion = moduleVersion;
            Manager = manager;
            Dependencies = dependencies.ToList();
        }
    }
}