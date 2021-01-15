using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Modules.ModuleManagers
{

    public abstract class ModuleManager : VisBase
    {

        public const string MODULE_LIST = "index.json";
        public const string MODULE_DATA = "module.zip";
        public const string MODULE_TARGET = "module.json";
        public const string MODULE_PATH = "modules";
        public readonly Uri ModuleRoot;

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public static void CreateModuleTarget( string targetFile )
        {
            File.WriteAllText(
                              targetFile,
                              JsonConvert.SerializeObject(
                                                          new ProjectConfig(
                                                                            null,
                                                                            "NewModule",
                                                                            "0.1",
                                                                            new ProjectDependency[0]
                                                                           ),
                                                          Formatting.Indented
                                                         )
                             );
        }

        public static string GetTargetDataPath( string root, string moduleName, string moduleVersion )
        {
            return Path.Combine( root, MODULE_PATH, moduleName, moduleVersion );
        }

        public abstract void Get( ProjectConfig target, string targetDir );

        public abstract string GetModulePackagePath( ModulePackage package );

        public abstract ModulePackage GetPackage( string name );

        public abstract IEnumerable < ModulePackage > GetPackages();

        public abstract string GetTargetDataPath( ProjectConfig target );

        public abstract string GetTargetDataUri( ProjectConfig target );

        public abstract string GetTargetInfoUri( ModulePackage package, string moduleVersion );

        public abstract bool HasPackage( string name );

        public abstract void Restore( ProjectConfig target, string rootDir );

        public virtual void AddPackage( ProjectConfig target, string moduleDataPath )
        {
            EventManager < ErrorEvent >.SendEvent( new ModuleManagerUnsupportedEvent( this, "Adding Packages" ) );
        }

        public bool HasTarget( ProjectConfig target )
        {
            return HasPackage( target.ProjectName ) &&
                   GetPackage( target.ProjectName ).HasTarget( target.ProjectVersion );
        }

        public virtual void RemovePackage( string moduleName )
        {
            EventManager < ErrorEvent >.SendEvent( new ModuleManagerUnsupportedEvent( this, "Removing Packages" ) );
        }

        public virtual void RemoveTarget( string moduleName, string moduleVersion )
        {
            EventManager < ErrorEvent >.SendEvent( new ModuleManagerUnsupportedEvent( this, "Removing Targets" ) );
        }

        #endregion

        #region Protected

        protected ModuleManager( string moduleRoot )
        {
            ModuleRoot = new Uri( moduleRoot, UriKind.Absolute );
        }

        protected static string GetPackageListPath( string root )
        {
            return Path.Combine(
                                root,
                                MODULE_LIST
                               );
        }

        protected string GetPackageListPath()
        {
            return GetPackageListPath(
                                      ModuleRoot.OriginalString
                                     );
        }

        protected List < ModulePackage > LoadPackageList()
        {
            string modListPath = GetPackageListPath();

            List < ModulePackage > ret = JsonConvert.DeserializeObject < List < ModulePackage > >(
                 File.ReadAllText( modListPath )
                );

            ret.ForEach( x => x.Manager = this );

            return ret;
        }

        protected List < ModulePackage > LoadPackageList( string root )
        {
            string modListPath = GetPackageListPath( root );

            List < ModulePackage > ret = JsonConvert.DeserializeObject < List < ModulePackage > >(
                 File.ReadAllText( modListPath )
                );

            ret.ForEach( x => x.Manager = this );

            return ret;
        }

        protected void SavePackageList( List < ModulePackage > packageList )
        {
            string modListPath = GetPackageListPath();

            File.WriteAllText(
                              modListPath,
                              JsonConvert.SerializeObject( packageList, Formatting.Indented )
                             );
        }

        #endregion

    }

}
