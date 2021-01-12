using System.Collections.Generic;
using System.Linq;

using VisCPU.Console.Core.Settings;
using VisCPU.Console.Core.Subsystems;
using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Console.Core
{

    public abstract class ConsoleSystem : ConsoleSubsystem
    {

        public abstract Dictionary < string, ConsoleSubsystem > SubSystems { get; }

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            CLISettings s = CLISettings.Create();
            ArgumentSyntaxParser.Parse( args.ToArray(), s );
            Dictionary < string, ConsoleSubsystem > ss = SubSystems;
            ss["help"] = new HelpSubSystem( this );
            VisConsole.RunConsole( s, args.ToArray(), ss );
        }

        #endregion

    }

}
