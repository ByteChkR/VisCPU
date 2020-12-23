using System.Collections.Generic;
using System.Linq;
using VisCPU.Console.Core.Settings;
using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Console.Core.Subsystems.Origins
{
    public class OriginSubSystem : ConsoleSystem
    {
        public override Dictionary<string, ConsoleSubsystem> SubSystems =>
            new Dictionary<string, ConsoleSubsystem>
            {
                {"add", new AddOriginSubSystem()},
                {"remove", new RemoveOriginSubSystem()},
                {"refresh", new RefreshOriginSubSystem()},
                {"list", new ListOriginSubSystem()}
            };
    }
}