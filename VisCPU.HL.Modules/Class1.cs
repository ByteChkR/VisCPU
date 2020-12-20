using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;

using Newtonsoft.Json;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.HL.Modules
{

    public class ModuleResolverSettings
    {

        [Argument(Name = "module.local")]
        public string LocalModuleRoot = "config/module/local";

    }
    public static class ModuleResolver
    {

        public static ModuleResolverSettings ResolverSettings;
        public static ModuleManager Manager;
        static ModuleResolver()
        {
            Initialize();
        }

        public static void Initialize()
        {
            if (ResolverSettings == null && Manager == null)
            {

                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config/module"));

                Settings.RegisterDefaultLoader(
                                               new JSONSettingsLoader(),
                                               Path.Combine(
                                                            AppDomain.CurrentDomain.BaseDirectory,
                                                            "config/module/module_resolver.json"
                                                           ),
                                               new ModuleResolverSettings()
                                              );
                ResolverSettings = Settings.GetSettings<ModuleResolverSettings>();
                Manager = new LocalModuleManager(ResolverSettings.LocalModuleRoot);
            }
        }


        public static ModuleTarget Resolve(ModuleDependency dependency)
        {
            return Manager.GetPackage(dependency.ModuleName).
                           GetInstallTarget(dependency.ModuleVersion == "ANY" ? null : dependency.ModuleVersion);
        }
    }

    [Serializable]
    public class ModulePackage
    {

        public string ModuleName;
        public List<string> ModuleVersions;
        [JsonIgnore, XmlIgnore]
        public ModuleManager Manager;
        public ModulePackage() { }
        public ModulePackage(ModuleManager manager, string moduleName, string[] moduleVersions)
        {
            ModuleName = moduleName;
            ModuleVersions = moduleVersions.ToList();
            Manager = manager;
        }

        public bool HasTarget(string version)
        {
            return ModuleVersions.Any(x => x == version);
        }

        public ModuleTarget GetInstallTarget(string version = null)
        {
            if (version != null)
            {
                string infoPath = Manager.GetTargetInfoUri(this, version);
                return JsonConvert.DeserializeObject<ModuleTarget>(
                                                                    File.ReadAllText(infoPath)
                                                                   );
            }
            return JsonConvert.DeserializeObject<ModuleTarget>(
                                                               File.ReadAllText(Manager.GetTargetInfoUri(this, ModuleVersions.Last()))
                                                              );
        }

    }

    [Serializable]
    public struct ModuleDependency
    {
        public string ModuleName;
        public string ModuleVersion;
    }

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

    public class LocalModuleManager : ModuleManager
    {

        private readonly List<ModulePackage> packageList;
        public LocalModuleManager(string moduleRoot) : base(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, moduleRoot))
        {
            Directory.CreateDirectory(ModuleRoot.OriginalString);
            string modListPath = Path.Combine(
                                              ModuleRoot.OriginalString,
                                              MODULE_LIST
                                             );

            if (!File.Exists(modListPath))
            {
                File.WriteAllText(modListPath, JsonConvert.SerializeObject(new List<ModulePackage>()));
            }

            packageList =
                JsonConvert.DeserializeObject < List < ModulePackage > >(
                                                                         File.ReadAllText( modListPath )

                                                                        );
            packageList.ForEach(x => x.Manager = this);
        }

        public override IEnumerable<ModulePackage> GetPackages()
        {
            return packageList;
        }

        public override bool HasPackage(string name)
        {
            return packageList.Any(x => x.ModuleName == name);
        }

        public override ModulePackage GetPackage(string name)
        {
            return packageList.First(x => x.ModuleName == name);
        }

        public override string GetTargetDataUri(ModuleTarget target)
        {
            return Path.Combine(GetTargetDataPath(target), MODULE_DATA);
        }
        public override string GetTargetInfoUri(ModulePackage package, string moduleVersion)
        {
            return Path.Combine(GetModulePackagePath(package), moduleVersion, MODULE_TARGET);
        }

        public override string GetTargetDataPath(ModuleTarget target)
        {
            return Path.Combine(ModuleRoot.OriginalString, MODULE_PATH, target.ModuleName, target.ModuleVersion);
        }
        public override string GetModulePackagePath(ModulePackage package)
        {
            return Path.Combine(ModuleRoot.OriginalString, MODULE_PATH, package.ModuleName);
        }

        public override void AddPackage(ModuleTarget target, string moduleDataPath)
        {
            ModulePackage package;
            
            
            if (! HasPackage( target.ModuleName ) )
            {
                package = new ModulePackage( this, target.ModuleName, new string[0] );
                packageList.Add(package);
            }
            else
            {
                package = GetPackage( target.ModuleName );
            }
            
            if (package.HasTarget(target.ModuleVersion))
            {
                throw new AccessViolationException("Can not Overwrite Existing Version");
            }
            package.ModuleVersions.Add(target.ModuleVersion);
            string data = GetTargetDataPath( target );
            Directory.CreateDirectory(data);
            File.Copy(moduleDataPath, GetTargetDataUri(target));
            SaveModuleTarget( target, GetTargetInfoUri( package, target.ModuleVersion ) );
            SavePackageList();
        }

        public override void Get(ModuleTarget target, string targetDir)
        {
            string dataPath = GetTargetDataUri(target);
            ZipFile.ExtractToDirectory(dataPath, targetDir);


            foreach (ModuleDependency targetDependency in target.Dependencies)
            {
                Get(
                    ModuleResolver.Resolve(targetDependency),
                    Path.Combine(targetDir, targetDependency.ModuleName)
                   );
            }

        }

        public override void Restore(ModuleTarget target, string rootDir)
        {
            foreach (ModuleDependency targetDependency in target.Dependencies)
            {
                Get(
                    ModuleResolver.Resolve(targetDependency),
                    Path.Combine(rootDir, targetDependency.ModuleName)
                   );
            }

        }

        public override void RemovePackage(string moduleName)
        {
            ModulePackage p = GetPackage(moduleName);
            Directory.Delete(GetModulePackagePath(p), true);
            packageList.Remove(p);
            SavePackageList();
        }

        private void SavePackageList()
        {
            string modListPath = Path.Combine(
                                              ModuleRoot.OriginalString,
                                              MODULE_LIST
                                             );
            File.WriteAllText(modListPath,
                JsonConvert.SerializeObject(packageList, Formatting.Indented));
        }

        public override void RemoveTarget(string moduleName, string moduleVersion)
        {
            ModulePackage p = GetPackage(moduleName);
            ModuleTarget t = p.GetInstallTarget(moduleVersion);
            string dataPath = GetTargetDataPath(t);

            if (Directory.Exists(dataPath))
                Directory.Delete(dataPath, true);

            p.ModuleVersions.RemoveAll(x => x == moduleVersion);
            SavePackageList();
        }

    }

    public abstract class ModuleManager
    {

        public static void CreateModuleTarget(string targetFile)
        {
            File.WriteAllText(
                              targetFile,
                              JsonConvert.SerializeObject(new ModuleTarget(null, "NewModule", "0.1", new ModuleDependency[0]), Formatting.Indented)
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

        public const string MODULE_LIST = "index.json";
        public const string MODULE_DATA = "module.zip";
        public const string MODULE_TARGET = "module.json";
        public const string MODULE_PATH = "modules";
        public readonly Uri ModuleRoot;

        protected ModuleManager(string moduleRoot)
        {
            ModuleRoot = new Uri(moduleRoot, UriKind.Absolute);
        }

        public abstract IEnumerable<ModulePackage> GetPackages();
        public abstract bool HasPackage(string name);
        public abstract ModulePackage GetPackage(string name);

        public abstract string GetTargetDataUri(ModuleTarget target);
        public abstract string GetTargetInfoUri(ModulePackage package, string moduleVersion);

        public abstract string GetModulePackagePath(ModulePackage package);

        public abstract string GetTargetDataPath(ModuleTarget target);


        public abstract void Get(ModuleTarget target, string targetDir);
        public abstract void Restore(ModuleTarget target, string rootDir);

        public virtual void AddPackage(ModuleTarget target, string moduleDataPath)
        {
            throw new NotSupportedException($"'{this}' does not support Adding Packages");
        }

        public virtual void RemovePackage(string moduleName)
        {
            throw new NotSupportedException($"'{this}' does not support Removing Packages");
        }
        public virtual void RemoveTarget(string moduleName, string moduleVersion)
        {
            throw new NotSupportedException($"'{this}' does not support Removing Targets");
        }
    }

}
