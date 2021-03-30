using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using VisCPU.Utility;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.ProjectSystem.Resolvers;
using VisCPU.Utility.ProjectSystem.UploadService;

namespace VisCPU.Console.Core.Subsystems.Origins.UploadService
{

    public class UploadServerSubSystem : ConsoleSubsystem
    {
        [field: Argument( Name = "port" )]
        public int Port { get; set; } = 21212;

        #region Public

        public override void Help()
        {
            HelpSubSystem.WriteSubsystem( "vis origin host <targetRepo>", this );
        }

        public override void Run( IEnumerable < string > args )
        {
            ArgumentSyntaxParser.Parse( args.Skip( 1 ).ToArray(), this );

            TcpProjectDatabaseServer server = new TcpProjectDatabaseServer(
                ProjectResolver.GetManager( args.First() ),
                Port,
                Path.Combine(
                    AppRootHelper.AppRoot,
                    "cache/upload_server"
                )
            );

            Thread t = new Thread( server.ServerLoop );
            t.Start();

            while ( true )
            {
                System.Console.WriteLine( "'exit' to close server" );
                System.Console.Write( ">" );

                string text = System.Console.ReadLine();

                if ( text.ToLower() == "exit" )
                {
                    break;
                }
            }

            server.Stop();
        }

        #endregion
    }

}
