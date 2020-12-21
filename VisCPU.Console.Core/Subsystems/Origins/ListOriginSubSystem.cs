using System.Collections.Generic;
using VisCPU.Console.Core.Settings;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems.Origins
{
    public class ListOriginSubSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
            OriginSettings s = SettingsSystem.GetSettings<OriginSettings>();

            foreach (KeyValuePair<string, string> keyValuePair in s.origins)
            {
                Log($"{keyValuePair.Key} : {keyValuePair.Value}");
            }
        }

    }
}