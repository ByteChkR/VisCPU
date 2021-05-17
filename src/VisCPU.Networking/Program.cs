using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using VisCPU.Peripherals.Network;
using VisCPU.Peripherals.Network.Internal;
using VisCPU.Utility.Logging;

namespace VisCPU.Networking
{
    class Program
    {
        private struct Command
        {
            private CommandFunc CommandFunction;
            private int ArgCount;
            public readonly string HelpText;
            public readonly string CommandName;

            public Command(string name, CommandFunc fnc, string helpText, int argCount)
            {
                CommandName = name;
                CommandFunction = fnc;
                HelpText = helpText;
                ArgCount = argCount;
            }

            public bool TryExecute(string[] args)
            {
                if (ArgCount != args.Length)
                {
                    Console.WriteLine("Expected {0} arguments but got {1} for Command '{2}'", ArgCount, args.Length, CommandName);
                    return false;
                }
                try
                {
                    CommandFunction(args);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }
        private delegate void CommandFunc(string[] args);

        private static void ListAdapterNames(LocalNetworkNode node)
        {
            NetworkAdapter[] entries = node.GetAllAdapters();
            Console.WriteLine("Adapters: ");

            for (int i = 0; i < entries.Length; i++)
            {
                NetworkAdapter entry = entries[i];
                Console.WriteLine("{0}:\n\tEntry Address: {1}\n\tEntry GUID: {2}\n\tEntry Connected: {3}",
                    i, entry.NetworkAddress, entry.GUID, entry.IsConnected);
            }
        }

        private static NetworkAdapter Find(LocalNetworkNode node, string guid)
        {
            return node.GetAllAdapters().FirstOrDefault(x => x.GUID.StartsWith(guid));
        }

        private static void ListAdapterPorts(LocalNetworkNode node, string guid)
        {
            NetworkAdapter adapter = Find(node, guid);
            NetworkPortListener[] entries = adapter.GetAllPortListeners();
            Console.WriteLine("Ports for Adapter: {0}", guid);

            for (int i = 0; i < entries.Length; i++)
            {
                NetworkPortListener entry = entries[i];
                Console.WriteLine("{0}\n\tIs Open: {1}\n\tPacket Count: {2}",
                    entry.GetIdentity(), entry.IsOpen, entry.PacketCount);
            }
        }
        private static void ListActiveTunnels(NetworkTunnelService service)
        {
            NetworkTunnel[] entries = service.GetActiveNetworkTunnels();
            Console.WriteLine("Active Tunnels: ");

            for (int i = 0; i < entries.Length; i++)
            {
                NetworkTunnel entry = entries[i];
                StringBuilder sb = new StringBuilder();

                int[] remotes = entry.GetRemoteAdapters();
                if (remotes.Length != 0)
                {
                    sb.Append(remotes[0]);
                    for(int adapterIndex = 1; adapterIndex < remotes.Length; adapterIndex++)
                    {
                        sb.AppendFormat(", {0}", remotes[i]);
                    }
                }

                Console.WriteLine("{0}\n\tIs Running: {1}\n\tEndpoint: {2}\n\tRemote Adapters: {3}",
                    i, entry.IsRunning, entry.Endpoint, sb);
            }
        }

        private static void ListDnsNames(DNSNetworkAdapter ad)
        {
            DNSNetworkAdapter.DNSEntry[] entries = ad.GetEntries();
            Console.WriteLine("DNS Entries: ");

            for (int i = 0; i < entries.Length; i++)
            {
                DNSNetworkAdapter.DNSEntry entry = entries[i];
                Console.WriteLine("{0}:\n\tEntry Name: {1}\n\tEntry Address: {2}\n\tEntry GUID: {3}",
                    i, entry.Name, entry.CurrentAddress, entry.GUID);
            }
        }

        static void Main(string[] args)
        {
            Dictionary<string, Command> cmds = new Dictionary<string, Command>();

            Utility.AppRootHelper.SetAppDomainBase();
            Logger.OnLogReceive += (x, y) => System.Console.WriteLine($"[{x}] {y}");
            int port = args.Length > 0 ? int.Parse(args[0]) : 42069;
            LocalNetworkNode node = new LocalNetworkNode();
            NetworkTunnelService tService = new NetworkTunnelService(node, port);


            cmds.Add("dns-names", new Command("dns-names", (s) => ListDnsNames(node.DNSAdapter), "Lists all known hostnames", 0));
            cmds.Add("adapters", new Command("adapters", (s) => ListAdapterNames(node), "Lists all known adatpers", 0));
            cmds.Add("ports", new Command("ports", (s) => ListAdapterPorts(node, s[0]), "Lists all Ports of the Adapter specified by partical or complete GUID", 1));
            cmds.Add("clear", new Command("clear", (s) => Console.Clear(), "Clears the Console", 0));
            cmds.Add("tunnels", new Command("tunnels", (s) => ListActiveTunnels(tService), "Lists all Active Tunnel Connections", 0));


            tService.Start();
            Thread.Sleep(1000);
            string cmd = null;
            Console.WriteLine("Type 'help' to display a list of commands.");
            while (true)
            {
                Console.Write("visnet> ");
                cmd = Console.ReadLine();
                if(cmd.ToLower() == "help")
                {
                    Console.WriteLine("Commands: ");
                    Console.WriteLine("\texit => Exits visnet");
                    Console.WriteLine("\thelp => Displays this Help Text");
                    foreach (KeyValuePair<string, Command> kvp in cmds)
                    {
                        Console.WriteLine("\t{0} => {1}", kvp.Key, kvp.Value.HelpText);
                    }
                }
                else if (cmd.ToLower() == "exit")
                {
                    break;
                }

                string[] ar = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if(ar.Length == 0 || !cmds.ContainsKey(ar[0]))
                {
                    Console.WriteLine("Invalid Command: " + cmd);
                }
                else
                {
                    Command command = cmds[ar[0]];
                    if(!command.TryExecute(ar.Skip(1).ToArray()))
                    {
                        Console.WriteLine("Command Failed.");
                    }
                }
            }
            node.UnloadNode();
            tService.Stop();
        }
    }
}
