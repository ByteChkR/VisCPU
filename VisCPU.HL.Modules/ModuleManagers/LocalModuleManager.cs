﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.Resolvers;

namespace VisCPU.HL.Modules.ModuleManagers
{
    public class LocalModuleManager : ModuleManager
    {
        private readonly List<ModulePackage> packageList;

        public LocalModuleManager(string moduleRoot) : base(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            moduleRoot))
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
                JsonConvert.DeserializeObject<List<ModulePackage>>(
                    File.ReadAllText(modListPath)
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


            if (!HasPackage(target.ModuleName))
            {
                package = new ModulePackage(this, target.ModuleName, new string[0]);
                packageList.Add(package);
            }
            else
            {
                package = GetPackage(target.ModuleName);
            }

            if (package.HasTarget(target.ModuleVersion))
            {
                throw new AccessViolationException("Can not Overwrite Existing Version");
            }
            package.ModuleVersions.Add(target.ModuleVersion);
            string data = GetTargetDataPath(target);
            Directory.CreateDirectory(data);
            File.Copy(moduleDataPath, GetTargetDataUri(target));
            SaveModuleTarget(target, GetTargetInfoUri(package, target.ModuleVersion));
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
            {
                Directory.Delete(dataPath, true);
            }

            p.ModuleVersions.RemoveAll(x => x == moduleVersion);
            SavePackageList();
        }
    }
}