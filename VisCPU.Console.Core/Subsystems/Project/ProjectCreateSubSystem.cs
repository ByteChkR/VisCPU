using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.BuildSystem;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ProjectCreateSubSystem : ConsoleSubsystem
    {

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            string[] a = args.ToArray();

            string path = a.Length != 0
                              ? Path.GetFullPath( a[0] )
                              :  Directory.GetCurrentDirectory();

            Log( $"Writing Project Info: {path}" );

            CommonFiles.InitializeProjectFolder( path );
            
            //ModuleManager.CreateModuleTarget( path );
        }

        public override void Help()
        {
            Log( "vis project create <projectRoot>" );
        }

        #endregion

    }

}
