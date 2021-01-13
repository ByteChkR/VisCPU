using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ModuleRestoreSubSystem : ConsoleSubsystem
    {

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            string[] a = args.ToArray();

            string repo = a.Length != 0? a[0]: "local";

            string root = a.Length > 1
                              ? Path.GetFullPath(a[1])
                              : Directory.GetCurrentDirectory();

            ModuleCleanSubSystem.Clean(root);

            string src = Path.Combine( root, "project.json" );

            ModuleTarget t =
                ModuleManager.LoadModuleTarget( src );

            ModuleResolver.GetManager(repo).Restore( t, root );
        }

        #endregion

    }

}
