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

            string repo = a[0];

            string root = a.Length > 1
                              ? Path.GetFullPath( a[1] )
                              : Directory.GetCurrentDirectory();

            string src = Path.Combine( root, "build", "module.json" );

            Log( "Packing '{0}'", src );
            ModulePackSubSystem.Pack( args.Skip(1) );

            if ( !File.Exists( src ) )
            {
                EventManager < ErrorEvent >.SendEvent( new FileNotFoundEvent( src, false ) );

                return;
            }

            Log("Publishing '{0}'", src);
            ModuleTarget t = ModuleManager.LoadModuleTarget( src );

            ModuleResolver.GetManager(repo).AddPackage(
                                              t,
                                              Path.Combine( root, "build", "module.zip" )
                                             );
        }

        #endregion

    }

}
