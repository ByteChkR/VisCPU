using System;
using System.IO;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.Utility.Settings;

namespace VisCPU.HL.Modules.Resolvers
{
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

                SettingsSystem.RegisterDefaultLoader(
                    new JSONSettingsLoader(),
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "config/module/module_resolver.json"
                    ),
                    new ModuleResolverSettings()
                );
                ResolverSettings = SettingsSystem.GetSettings<ModuleResolverSettings>();
                Manager = new LocalModuleManager(ResolverSettings.LocalModuleRoot);
            }
        }


        public static ModuleTarget Resolve(ModuleDependency dependency)
        {
            return Manager.GetPackage(dependency.ModuleName)
                .GetInstallTarget(dependency.ModuleVersion == "ANY" ? null : dependency.ModuleVersion);
        }
    }
}