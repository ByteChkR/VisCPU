using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using Newtonsoft.Json;

using VisCPU.HL.Modules.ModuleManagers;

namespace VisCPU.HL.Modules.Data
{

    [Serializable]
    public class ModulePackage
    {

        [JsonIgnore]
        [XmlIgnore]
        public ModuleManager Manager;

        public string ModuleName;
        public List < string > ModuleVersions;

        #region Public

        public ModulePackage()
        {
        }

        public ModulePackage( ModuleManager manager, string moduleName, string[] moduleVersions )
        {
            ModuleName = moduleName;
            ModuleVersions = moduleVersions.ToList();
            Manager = manager;
        }

        public ProjectConfig GetInstallTarget( string version = null )
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
