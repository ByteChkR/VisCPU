using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VisCPU.Utility.ProjectSystem.Database;

namespace VisCPU.Utility.ProjectSystem.Data
{

    [Serializable]
    public class ProjectPackage
    {
        [JsonIgnore]
        [XmlIgnore]
        public ProjectDatabase Manager { get; set; }

        public string ModuleName { get; set; }

        public List < string > ModuleVersions { get; set; }

        #region Public

        public ProjectPackage()
        {
        }

        public ProjectPackage( ProjectDatabase manager, string moduleName, string[] moduleVersions )
        {
            ModuleName = moduleName;
            ModuleVersions = moduleVersions.ToList();
            Manager = manager;
        }

        public ProjectConfig GetInstallTarget()
        {
            return GetInstallTarget( null );
        }

        public ProjectConfig GetInstallTarget( string version )
        {
            if ( version != null )
            {
                string infoPath = Manager.GetTargetInfoUri( this, version );

                return JsonConvert.DeserializeObject < ProjectConfig >(
                    File.ReadAllText( infoPath )
                );
            }

            return JsonConvert.DeserializeObject < ProjectConfig >(
                File.ReadAllText(
                    Manager.GetTargetInfoUri(
                        this,
                        ModuleVersions.Last()
                    )
                )
            );
        }

        public bool HasTarget( string version )
        {
            return ModuleVersions.Any( x => x == version );
        }

        #endregion
    }

}
