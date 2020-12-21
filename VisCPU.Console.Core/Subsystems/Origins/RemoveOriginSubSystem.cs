using System.Collections.Generic;
using System.Linq;
using VisCPU.Console.Core.Settings;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems.Origins
{
    public class RemoveOriginSubSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
            OriginSettings s = Settings.GetSettings<OriginSettings>();
            string name = args.First();
            s.origins.Remove(name);
            Settings.SaveSettings(s);
        }

    }
}