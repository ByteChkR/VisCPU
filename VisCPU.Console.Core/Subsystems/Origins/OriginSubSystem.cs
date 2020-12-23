using System.Collections.Generic;
using System.Linq;
using VisCPU.Console.Core.Settings;
using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Console.Core.Subsystems.Origins
{
    public class OriginSubSystem : ConsoleSubsystem
    {
        private readonly Dictionary<string, ConsoleSubsystem> subsystems =
            new Dictionary<string, ConsoleSubsystem>
            {
                {"add", new AddOriginSubSystem()},
                {"remove", new RemoveOriginSubSystem()},
                {"refresh", new RefreshOriginSubSystem()},
                {"list", new ListOriginSubSystem()}
            };

        public override void Run(IEnumerable<string> args)
        {
            CLISettings s = CLISettings.Create();
            ArgumentSyntaxParser.Parse(args.ToArray(), s);
            VisConsole.RunConsole(s, args.ToArray(), subsystems);
        }
    }
}