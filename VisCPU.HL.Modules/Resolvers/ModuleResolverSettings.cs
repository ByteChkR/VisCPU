using System;
using System.Collections.Generic;
using System.IO;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.HL.Modules.Resolvers
{

    public class ModuleResolverSettings
    {

        [field: Argument( Name = "module.origins" )]
        public Dictionary < string, string > ModuleOrigins { get; set; } =
            new Dictionary < string, string > { { "local", "config/module/local" } };

        #region Public

        public static ModuleResolverSettings Create()
        {
            return SettingsSystem.GetSettings < ModuleResolverSettings >();
        }

        #endregion

        #region Private

        static ModuleResolverSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                                                 new JSONSettingsLoader(),
                                                 Path.Combine(
                                                              AppDomain.CurrentDomain.BaseDirectory,
                                                              "config/module/module_resolver.json"
                                                             ),
                                                 new ModuleResolverSettings()
                                                );
        }

        #endregion

    }

}
