using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VisCPU.HL.Modules.Data;

namespace VisCPU.HL.Modules.ModuleManagers
{
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