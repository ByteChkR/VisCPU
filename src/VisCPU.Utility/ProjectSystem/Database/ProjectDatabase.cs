using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.Logging;
using VisCPU.Utility.ProjectSystem.Data;
using VisCPU.Utility.ProjectSystem.Database.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Utility.ProjectSystem.Database
{

    public abstract class ProjectDatabase : VisBase
    {
        public static readonly string s_ModuleList = "index.json";
        public static readonly string s_ModuleData = "module.zip";
        public static readonly string s_ModuleTarget = "module.json";
        public static readonly string s_ModulePath = "modules";

        public Uri ModuleRoot { get; }

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public static string GetTargetDataPath( string root, string moduleName, string moduleVersion )
        {
            return Path.Combine( root, s_ModulePath, moduleName, moduleVersion );
        }

        public abstract void Get( ProjectConfig target, string targetDir );

        public abstract string GetModulePackagePath( ProjectPackage package );

        public abstract ProjectPackage GetPackage( string name );

        public abstract IEnumerable < ProjectPackage > GetPackages();

        public abstract string GetTargetDataPath( ProjectConfig target );

        public abstract string GetTargetDataUri( ProjectConfig target );

        public abstract string GetTargetInfoUri( ProjectPackage package, string moduleVersion );

        public abstract bool HasPackage( string name );

        public abstract void Restore( ProjectConfig target, string rootDir );

        public virtual void AddPackage( ProjectConfig target, string moduleDataPath )
        {
            EventManager < ErrorEvent >.SendEvent( new ProjectDatabaseUnsupportedEvent( this, "Adding Packages" ) );
        }

        public bool HasTarget( ProjectConfig target )
        {
            return HasPackage( target.ProjectName ) &&
                   GetPackage( target.ProjectName ).HasTarget( target.ProjectVersion );
        }

        public virtual void RemovePackage( string moduleName )
        {
            EventManager < ErrorEvent >.SendEvent( new ProjectDatabaseUnsupportedEvent( this, "Removing Packages" ) );
        }

        public virtual void RemoveTarget( string moduleName, string moduleVersion )
        {
            EventManager < ErrorEvent >.SendEvent( new ProjectDatabaseUnsupportedEvent( this, "Removing Targets" ) );
        }

        #endregion

        #region Protected

        protected ProjectDatabase( string moduleRoot )
        {
            ModuleRoot = new Uri( moduleRoot, UriKind.Absolute );
        }

        protected static string GetPackageListPath( string root )
        {
            return Path.Combine(
                root,
                s_ModuleList
            );
        }

        protected string GetPackageListPath()
        {
            return GetPackageListPath(
                ModuleRoot.OriginalString
            );
        }

        protected List < ProjectPackage > LoadPackageList()
        {
            string modListPath = GetPackageListPath();

            List < ProjectPackage > ret = JsonConvert.DeserializeObject < List < ProjectPackage > >(
                File.ReadAllText( modListPath )
            );

            ret.ForEach( x => x.Manager = this );

            return ret;
        }

        protected List < ProjectPackage > LoadPackageList( string root )
        {
            string modListPath = GetPackageListPath( root );

            List < ProjectPackage > ret = JsonConvert.DeserializeObject < List < ProjectPackage > >(
                File.ReadAllText( modListPath )
            );

            ret.ForEach( x => x.Manager = this );

            return ret;
        }

        protected void SavePackageList( List < ProjectPackage > packageList )
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
