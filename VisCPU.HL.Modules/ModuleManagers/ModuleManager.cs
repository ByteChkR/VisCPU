using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Newtonsoft.Json;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers.Events;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Modules.ModuleManagers
{

    public abstract class ModuleManager : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        public const string MODULE_LIST = "index.json";
        public const string MODULE_DATA = "module.zip";
        public const string MODULE_TARGET = "module.json";
        public const string MODULE_PATH = "modules";
        public readonly Uri ModuleRoot;

        #region Public

        protected static string GetPackageListPath(string root)
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

        protected List<ModulePackage> LoadPackageList()
        {
            string modListPath = GetPackageListPath();

            List<ModulePackage> ret = JsonConvert.DeserializeObject<List<ModulePackage>>(
                 File.ReadAllText(modListPath)
                );

            ret.ForEach(x => x.Manager = this);
            return ret;
        }

        protected List<ModulePackage> LoadPackageList(string root)
        {
            string modListPath = GetPackageListPath(root);

            List < ModulePackage > ret = JsonConvert.DeserializeObject < List < ModulePackage > >(
                 File.ReadAllText( modListPath )
                );

            ret.ForEach( x => x.Manager = this );
            return ret;
        }

        protected void SavePackageList(List<ModulePackage> packageList)
        {
            string modListPath = GetPackageListPath();

            File.WriteAllText(
                              modListPath,
                              JsonConvert.SerializeObject(packageList, Formatting.Indented)
                             );
        }

        public static void CreateModuleTarget(string targetFile)
        {
            File.WriteAllText(
                              targetFile,
                              JsonConvert.SerializeObject(
                                                          new ModuleTarget(
                                                                           null,
                                                                           "NewModule",
                                                                           "0.1",
                                                                           new ModuleDependency[0]
                                                                          ),
                                                          Formatting.Indented
                                                         )
                             );
        }

        public static ModuleTarget LoadModuleTarget(string targetFile)
        {
            return JsonConvert.DeserializeObject<ModuleTarget>(File.ReadAllText(targetFile));
        }

        public static void SaveModuleTarget(ModuleTarget target, string targetFile)
        {
            File.WriteAllText(targetFile, JsonConvert.SerializeObject(target, Formatting.Indented));
        }

        public abstract void Get(ModuleTarget target, string targetDir);

        public abstract string GetModulePackagePath(ModulePackage package);

        public abstract ModulePackage GetPackage(string name);

        public abstract IEnumerable<ModulePackage> GetPackages();

        public abstract string GetTargetDataPath(ModuleTarget target);
        public static string GetTargetDataPath(string root, string moduleName, string moduleVersion)
        {
            return Path.Combine(root, MODULE_PATH, moduleName, moduleVersion);
        }

        public abstract string GetTargetDataUri(ModuleTarget target);

        public abstract string GetTargetInfoUri(ModulePackage package, string moduleVersion);

        public abstract bool HasPackage(string name);

        public bool HasTarget( ModuleTarget target )
        {
            return HasPackage( target.ModuleName ) && GetPackage( target.ModuleName ).HasTarget( target.ModuleVersion );
        }

        public abstract void Restore(ModuleTarget target, string rootDir);

        public virtual void AddPackage(ModuleTarget target, string moduleDataPath)
        {
            EventManager<ErrorEvent>.SendEvent(new ModuleManagerUnsupportedEvent(this, "Adding Packages"));
        }

        public virtual void RemovePackage(string moduleName)
        {
            EventManager<ErrorEvent>.SendEvent(new ModuleManagerUnsupportedEvent(this, "Removing Packages"));
        }

        public virtual void RemoveTarget(string moduleName, string moduleVersion)
        {
            EventManager<ErrorEvent>.SendEvent(new ModuleManagerUnsupportedEvent(this, "Removing Targets"));
        }

        #endregion

        #region Protected

        protected ModuleManager(string moduleRoot)
        {
            ModuleRoot = new Uri(moduleRoot, UriKind.Absolute);
        }

        #endregion

    }

    public class HttpModuleManager : ModuleManager
    {

        private readonly string RepoName;
        private readonly string LocalTempCache;
        private List<ModulePackage> pList;

        private List<ModulePackage> PackageList => pList ?? (pList = GetPackageList());

        public HttpModuleManager(string repoName, string moduleRoot) : base(moduleRoot)
        {
            RepoName = repoName;
            LocalTempCache = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "module", RepoName);
            Directory.CreateDirectory( LocalTempCache );
        }

        private List<ModulePackage> GetPackageList()
        {
            string repoIndex = GetPackageListPath();
            string localIndex = GetPackageListPath(LocalTempCache);

            if (File.Exists(localIndex))
                File.Delete(localIndex);

            using (WebClient wc = new WebClient())
            {
                Log("Downloading Index: {0}", repoIndex);
                wc.DownloadFile(new Uri(repoIndex), localIndex);
            }

            List <ModulePackage> packages= LoadPackageList(LocalTempCache);
            SaveInfo( packages );
            return packages;
        }

        private void SaveInfo(List <ModulePackage> packages)
        {
            foreach ( ModulePackage modulePackage in packages )
            {
                foreach ( string modulePackageModuleVersion in modulePackage.ModuleVersions )
                {
                    string infoLocal = GetTargetInfoUri(modulePackage, modulePackageModuleVersion);
                    string infoRemote = GetRemoteTargetInfoUri(modulePackage, modulePackageModuleVersion);
                    if (File.Exists(infoLocal))continue;


                    string dir = GetTargetDataPath(
                                                   LocalTempCache,
                                                   modulePackage.ModuleName,
                                                   modulePackageModuleVersion
                                                  );
                    Directory.CreateDirectory(
                                              dir
                                             );
                    using (WebClient wc = new WebClient())
                    {
                        Log("Downloading Info: {0}", infoRemote);
                        wc.DownloadFile(new Uri(infoRemote), infoLocal);
                    }

                }
            }
            
        }

        private void FetchData(ModuleTarget target)
        {
            string dataUri = GetRemoteTargetDataUri(target);
            string dataLocal = GetTargetDataUri(target);

            if ( File.Exists( dataLocal ) )
                return;

            Directory.CreateDirectory( GetTargetDataPath( target ) );
            using (WebClient wc = new WebClient())
            {
                Log("Downloading Module: {0}", dataUri);
                wc.DownloadFile(new Uri(dataUri), dataLocal);
            }
        }

        public override void Get(ModuleTarget target, string targetDir)
        {
            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
            }

            string dataPath = GetTargetDataUri(target);

            if (!File.Exists(dataPath))
            {
                FetchData(target);
            }

            ZipFile.ExtractToDirectory(dataPath, targetDir);

            foreach (ModuleDependency targetDependency in target.Dependencies)
            {
                Get(
                    ModuleResolver.Resolve(this, targetDependency),
                    Path.Combine(targetDir, targetDependency.ModuleName)
                   );
            }
        }
        private string GetRemoteModulePackagePath(ModulePackage package)
        {
            return Path.Combine(ModuleRoot.OriginalString, MODULE_PATH, package.ModuleName);
        }
        private string GetRemoteModulePackagePath(ModuleTarget package)
        {
            return Path.Combine(ModuleRoot.OriginalString, MODULE_PATH, package.ModuleName);
        }

        public override string GetModulePackagePath(ModulePackage package)
        {
            return Path.Combine(LocalTempCache, MODULE_PATH, package.ModuleName);
        }

        public override ModulePackage GetPackage(string name)
        {
            return PackageList.First(x => x.ModuleName == name);
        }

        public override IEnumerable<ModulePackage> GetPackages()
        {
            return PackageList;
        }

        private string GetRemoteTargetDataPath(ModuleTarget target)
        {
            return Path.Combine(ModuleRoot.OriginalString, MODULE_PATH, target.ModuleName, target.ModuleVersion);
        }

        private string GetRemoteTargetDataUri(ModuleTarget target)
        {
            return Path.Combine(GetRemoteTargetDataPath(target), MODULE_DATA);
        }

        private string GetRemoteTargetInfoUri(ModulePackage package, string moduleVersion)
        {
            return Path.Combine(GetRemoteModulePackagePath(package), moduleVersion, MODULE_TARGET);
        }

        private string GetRemoteTargetInfoUri(ModuleTarget target)
        {
            return Path.Combine(GetRemoteModulePackagePath(target), target.ModuleVersion, MODULE_TARGET);
        }

        

        public override string GetTargetDataPath(ModuleTarget target)
        {
            return GetTargetDataPath(LocalTempCache, target.ModuleName, target.ModuleVersion);
        }

        public override string GetTargetDataUri(ModuleTarget target)
        {
            return Path.Combine(GetTargetDataPath(target), MODULE_DATA);
        }

        public override string GetTargetInfoUri(ModulePackage package, string moduleVersion)
        {
            return Path.Combine(GetModulePackagePath(package), moduleVersion, MODULE_TARGET);
        }

        public override bool HasPackage(string name)
        {
            return PackageList.Any(x => x.ModuleName == name);
        }

        public override void Restore(ModuleTarget target, string rootDir)
        {
            foreach (ModuleDependency targetDependency in target.Dependencies)
            {
                Get(
                    ModuleResolver.Resolve(this, targetDependency),
                    Path.Combine(rootDir, targetDependency.ModuleName)
                   );
            }
        }

    }

    public class TCPUploadModuleManager : ModuleManager
    {

        public TCPUploadModuleManager( string moduleRoot ) : base( moduleRoot )
        {
        }

        public override void AddPackage( ModuleTarget target, string moduleDataPath )
        {
            TcpClient client = new TcpClient( ModuleRoot.Host, ModuleRoot.Port );

            byte[] mod = Encoding.UTF8.GetBytes( JsonConvert.SerializeObject( target ) );
            client.GetStream().Write( BitConverter.GetBytes( mod.Length ), 0, sizeof( int ) );
            client.GetStream().Write( mod, 0, mod.Length );
            FileInfo info = new FileInfo( moduleDataPath );
            client.GetStream().Write(BitConverter.GetBytes((int)info.Length), 0, sizeof(int));
            Stream fs = info.OpenRead();
            fs.CopyTo( client.GetStream() );
            fs.Close();
            byte[] response = new byte[sizeof( int )];
            client.GetStream().Read( response, 0, response.Length );
            int responseLen = BitConverter.ToInt32( response, 0 );
            response = new byte[responseLen];
            client.GetStream().Read(response, 0, response.Length);
            client.Close();
            Log( "Response: {0}", Encoding.UTF8.GetString( response ) );
        }

        public override void Get( ModuleTarget target, string targetDir )
        {
        }

        public override string GetModulePackagePath( ModulePackage package )
        {
            return null;
        }

        public override ModulePackage GetPackage( string name )
        {
            return null;
        }

        public override IEnumerable < ModulePackage > GetPackages()
        {
            yield break;
        }

        public override string GetTargetDataPath( ModuleTarget target )
        {
            return null;
        }

        public override string GetTargetDataUri( ModuleTarget target )
        {
            return null;
        }

        public override string GetTargetInfoUri( ModulePackage package, string moduleVersion )
        {
            return null;
        }

        public override bool HasPackage( string name )
        {
            return false;
        }

        public override void Restore( ModuleTarget target, string rootDir )
        {
        }

    }

}
