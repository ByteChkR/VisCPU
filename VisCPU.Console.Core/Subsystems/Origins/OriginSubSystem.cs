using System.Collections.Generic;
using VisCPU.Console.Core.Subsystems.Origins.UploadService;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    public class OriginSubSystem : ConsoleSystem
    {
        public override Dictionary < string, ConsoleSubsystem > SubSystems =>
            new Dictionary < string, ConsoleSubsystem >
            {
                { "add", new AddOriginSubSystem() },
                { "remove", new RemoveOriginSubSystem() },
                { "list", new ListOriginSubSystem() },
                { "host", new UploadServerSubSystem() },
                { "packages", new ListPackagesSubSystem() }
            };
    }

}
