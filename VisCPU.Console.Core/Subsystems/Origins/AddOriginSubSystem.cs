using System.Collections.Generic;
using System.Linq;

using VisCPU.Console.Core.Settings;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    public class AddOriginSubSystem : ConsoleSubsystem
    {

        #region Public

        public override void Run(IEnumerable<string> args)
        {
            ModuleResolverSettings s = ModuleResolverSettings.Create();
            string[] a = args.ToArray();
            string name = a[0];
            string url = a[1];
            s.ModuleOrigins[name] = url;
            SettingsSystem.SaveSettings(s);
        }

        #endregion

    }

}
