using System;
using System.Collections.Generic;
using System.IO;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.Utility.Settings;

namespace VisCPU.HL.Modules.Resolvers
{

    public static class ModuleResolver
    {

        public static ModuleResolverSettings ResolverSettings;

        private static Dictionary<string, ModuleManager> Managers;
        //public static ModuleManager Manager;

        #region Public

        public static IEnumerable<KeyValuePair<string, ModuleManager>> GetManagers() => Managers;

        public static void Initialize()
        {
            if (ResolverSettings == null && Managers == null)
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config/module"));

                

                ResolverSettings = ModuleResolverSettings.Create();
                Managers = new Dictionary<string, ModuleManager>();

                foreach (KeyValuePair<string, string> origin in ResolverSettings.ModuleOrigins)
                {
                    if (Uri.TryCreate(origin.Value, UriKind.Absolute, out Uri u))
                    {
                        if (u.Scheme == "http" || u.Scheme == "https")
                            Managers.Add(origin.Key, new HttpModuleManager(origin.Key, u.OriginalString));
                        else if (u.Scheme == "dev")
                        {
                            Managers.Add(origin.Key, new TCPUploadModuleManager(u.OriginalString));
                        }
                        else
                        {
                            Managers.Add(origin.Key, new LocalModuleManager(origin.Value));
                        }
                    }
                    else
                    {
                        Managers.Add(origin.Key, new LocalModuleManager(origin.Value));
                    }
                }

            }
        }

        public static ModuleManager GetManager(string name) => Managers[name];

        public static ModuleTarget Resolve(string repository, ModuleDependency dependency)
        {
            return Managers[repository].GetPackage(dependency.ModuleName).
                                        GetInstallTarget(dependency.ModuleVersion == "ANY" ? null : dependency.ModuleVersion);
        }
        public static ModuleTarget Resolve(ModuleManager manager, ModuleDependency dependency)
        {
            return manager.GetPackage(dependency.ModuleName).
                                        GetInstallTarget(dependency.ModuleVersion == "ANY" ? null : dependency.ModuleVersion);
        }

        #endregion

        #region Private

        static ModuleResolver()
        {
            Initialize();
        }

        #endregion

    }

}
