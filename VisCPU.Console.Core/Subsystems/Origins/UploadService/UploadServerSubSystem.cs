using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using VisCPU.HL.Modules.Resolvers;
using VisCPU.HL.Modules.UploadService;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems.Origins.UploadService
{


    public class UploadServerSubSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
            UploadServer server = new UploadServer( ModuleResolver.GetManager(args.First()), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache/upload_server"));
            Thread t = new Thread( server.ServerLoop );
            t.Start();

            while ( true )
            {
                System.Console.WriteLine("'exit' to close server");
                System.Console.Write(">");

                string text = System.Console.ReadLine();

                if ( text.ToLower() == "exit" )
                {
                    break;
                }
            }

            server.Stop();
        }

    }

}
