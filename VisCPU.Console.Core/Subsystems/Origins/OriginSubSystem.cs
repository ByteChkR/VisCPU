using System.Collections.Generic;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    public class OriginSubSystem : ConsoleSystem
    {

        public override Dictionary < string, ConsoleSubsystem > SubSystems =>
            new Dictionary < string, ConsoleSubsystem >
            {
                { "add", new AddOriginSubSystem() },
                { "remove", new RemoveOriginSubSystem() },
                { "refresh", new RefreshOriginSubSystem() },
                { "list", new ListOriginSubSystem() }
            };

    }

}
