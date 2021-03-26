using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using VisCPU.Utility.IO.Settings;

namespace VisCPU.Console.Core.Subsystems.VM
{

    [Serializable]
    public class VMConfigs
    {
        public List <VMConfig> Configurations = new List < VMConfig >{new VMConfig()};
    }

    [Serializable]
    public class VMConfig
    {
        public string Name = "Default";
        public string Root = "./";
    }


    public class VMStartSubSystem:ConsoleSubsystem
    {
        public override void Help()
        {
        }

        public override void Run( IEnumerable < string > args )
        {
            string name = args.First();
            VMConfigs vms = SettingsManager.GetSettings < VMConfigs >();
            VMConfig c = vms.Configurations.FirstOrDefault( x => x.Name == name );

            if ( c == null )
            {
                string p = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, name );
                Directory.CreateDirectory( p );
                c = new VMConfig() { Name = name, Root = p };
                vms.Configurations.Add( c );
                SettingsManager.SaveSettings( vms );
            }

            StringBuilder sb = new StringBuilder( $"--root {c.Root} " );

            foreach ( string s in args )
            {
                sb.Append( $" {s}" );
            }
            Process.Start( Process.GetCurrentProcess().MainModule.FileName, sb.ToString() );
        }
    }

    public class VMSubsystem: ConsoleSystem
    {
        public override Dictionary < string, ConsoleSubsystem > SubSystems { get; } =
            new Dictionary < string, ConsoleSubsystem > { { "start", new VMStartSubSystem() } };
    }
}
