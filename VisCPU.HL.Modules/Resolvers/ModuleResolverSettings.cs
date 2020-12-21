using VisCPU.Utility.ArgumentParser;

namespace VisCPU.HL.Modules.Resolvers
{

    public class ModuleResolverSettings
    {

        [Argument(Name = "module.local")]
        public string LocalModuleRoot = "config/module/local";

    }
}
