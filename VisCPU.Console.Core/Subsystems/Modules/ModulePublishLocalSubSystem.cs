using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ModulePublishLocalSubSystem : ConsoleSubsystem
    {

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            string[] a = args.ToArray();

            string root = a.Length != 0
                              ? Path.GetFullPath( a[0] )
                              : Directory.GetCurrentDirectory();

            string src = Path.Combine( root, "build", "module.json" );

            if ( !File.Exists( src ) )
            {
                EventManager < ErrorEvent >.SendEvent( new FileNotFoundEvent( src, false ) );

                return;
            }

            ModuleTarget t = ModuleManager.LoadModuleTarget( src );

            ModuleResolver.Manager.AddPackage(
                                              t,
                                              Path.Combine( root, "build", "module.zip" )
                                             );
        }

        #endregion

    }

}
