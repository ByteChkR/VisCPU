using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace VisCPU.Peripherals.Network.Internal
{
    public class EchoNetworkAdapter : NetworkAdapter
    {
        private const int ECHO_SERVER_PORT = 444;
        private bool m_Exit;
        private Thread m_Thread;
        public EchoNetworkAdapter(INetworkNode node) : base(node)
        {
            OnConnect += EchoNetworkAdapter_OnConnect;
            OnDisconnect += EchoNetworkAdapter_OnDisconnect;
        }

        private void EchoNetworkAdapter_OnDisconnect()
        {
            if (m_Thread == null || !m_Thread.IsAlive) return;
            m_Exit = true;
            while (m_Exit) { }
        }

        private void EchoNetworkAdapter_OnConnect()
        {
            EchoNetworkAdapter_OnDisconnect();
            m_Thread = new Thread(Loop);
            m_Thread.Start();
        }

        private void Loop()
        {
            Log("Starting Echo Server. Thead ID: " + Thread.CurrentThread.ManagedThreadId);
            NetworkPortListener listener = OpenPort(ECHO_SERVER_PORT);
            
            while (true)
            {
                if (m_Exit)
                {
                    break;
                }
                while (listener.PacketCount != 0)
                {
                    NetworkPacket packet = listener.ReadPacket();

                    packet.Respond(this, packet.Data); //Echo the Packet

                    StringBuilder sb = new StringBuilder();
                    foreach(uint data in packet.Data)
                    {
                        sb.Append((char)data);
                    }
                    Log("Echo Package Contents: {0}", sb);
                }

                Thread.Sleep(NetworkSettings.EchoServerThreadSleep);
            }
            m_Exit = false;
        }
    }
    public class DNSNetworkAdapter : NetworkAdapter
    {
        public class DNSEntry
        {
            public string Name;
            public string GUID;
            public int CurrentAddress;
        }

        private const int DNS_SERVER_PORT = 77;
        private const int DNS_CMD_RESOLVE = 1;
        private const int DNS_CMD_REGISTER = 2;
        private bool m_Exit;
        private string m_DefaultName;
        private Thread m_Thread;
        private ConcurrentDictionary<string, DNSEntry> nameMap = new ConcurrentDictionary<string, DNSEntry>();

        public DNSEntry[] GetEntries() => nameMap.Values.ToArray();

        public DNSNetworkAdapter(INetworkNode node, string name = null) : base(node) 
        {
            m_DefaultName = name;
            OnConnect += DNSNetworkAdapter_OnConnect;
            OnDisconnect += DNSNetworkAdapter_OnDisconnect;
        }

        private void DNSNetworkAdapter_OnDisconnect()
        {
            if (m_Thread == null || !m_Thread.IsAlive) return;
            m_Exit = true;
            while (m_Exit) { }
        }

        private void DNSNetworkAdapter_OnConnect()
        {
            DNSNetworkAdapter_OnDisconnect();
            m_Thread = new Thread(Loop);
            m_Thread.Start();
        }

        internal void Register(string name, string guid, int addr) => nameMap[name] = new DNSEntry {Name = name, CurrentAddress = addr, GUID = guid };

        private void Loop()
        {
            Log("Starting DNS Server. Thead ID: " + Thread.CurrentThread.ManagedThreadId);
            NetworkPortListener listener =  OpenPort(DNS_SERVER_PORT);
            if(m_DefaultName!=null)
            nameMap[m_DefaultName] = new DNSEntry { Name = m_DefaultName, CurrentAddress = NetworkAddress, GUID = GUID };
            while (true)
            {
                if(m_Exit)
                {
                    break;
                }
                while(listener.PacketCount != 0)
                {
                    NetworkPacket packet = listener.ReadPacket();
                    if (packet.Data.Length == 0) continue; //Drop Packet
                    uint cmd = packet.Data[0];
                    if(cmd == DNS_CMD_REGISTER && packet.Data.Length > 1)
                    {
                        string data = new string(packet.Data.Skip(1).Select(x => (char)x).ToArray());
                        string name = data.Remove(0, 36); //Remove GUID
                        string guid = data.Remove(36); //Remove Name
                        if (nameMap.TryGetValue(name, out DNSEntry e))
                        {
                            if(guid == e.GUID) //Update the Address.
                            {
                                e.CurrentAddress = packet.Sender.NetworkAddress;
                                packet.Respond(this, new[] { 1u });
                            }
                            else
                            {
                                packet.Respond(this, new[] { 0u });
                            }
                        }
                        else
                        {
                            packet.Respond(this, new[] { 1u });
                            nameMap[name] = new DNSEntry {Name = name, CurrentAddress = packet.Sender.NetworkAddress, GUID = guid };
                        }
                    }
                    else if(cmd == DNS_CMD_RESOLVE)
                    {
                        string name = new string(packet.Data.Skip(1).Select(x => (char)x).ToArray());
                        if (nameMap.ContainsKey(name))
                        {
                            packet.Respond(this, new[] { (uint)nameMap[name].CurrentAddress });
                        }
                        else
                        {
                            packet.Respond(this, new[] { 0u });
                        }
                    }
                }
                Thread.Sleep(NetworkSettings.DNSServerThreadSleep);
            }
            m_Exit = false;
        }
    }
}
