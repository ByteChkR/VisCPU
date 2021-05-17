using System;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace VisCPU.Peripherals.Network
{
    public class SlaveNetworkNode : NetworkBase, INetworkNode
    {
        private ConcurrentDictionary<int, NetworkAdapter> m_LocalAdapters = new ConcurrentDictionary<int, NetworkAdapter>();
        private readonly TcpClient m_Client;
        private Thread m_Thread;
        private bool m_Exit;
        public SlaveNetworkNode(string ip, int port)
        {
            m_Client = new TcpClient(ip, port);
            Start();
        }

        public void Start()
        {
            Stop();
            m_Thread = new Thread(Loop);
            m_Thread.Start();
        }

        public void Stop()
        {
            if (m_Thread == null || !m_Thread.IsAlive)
                return;
            m_Exit = true;
            while (m_Exit) { } //Wait for Thread to Exit
        }

        public void UnloadNode()
        {
            Stop();
        }

        private void Loop()
        {
            Log("Starting Slave Network Node. Thead ID: " + Thread.CurrentThread.ManagedThreadId);
            while (true)
            {
                try
                {

                    if (m_Exit || !m_Client.Connected)
                    {
                        break;
                    }
                    int av = 0;
                    lock (m_Client)
                        av = m_Client.Available;
                    if (av >= 1)
                    {
                        lock(m_Client)
                        {
                            NetworkNodeRequestType type = (NetworkNodeRequestType)m_Client.GetStream().ReadByte();
                            byte[] buf;
                            switch (type)
                            {
                                case NetworkNodeRequestType.SEND:
                                    {
                                        buf = new byte[sizeof(int)];
                                        m_Client.GetStream().Read(buf, 0, buf.Length);
                                        int len = BitConverter.ToInt32(buf, 0);
                                        buf = new byte[len];
                                        m_Client.GetStream().Read(buf, 0, buf.Length);
                                        NetworkPacket p = NetworkPacket.Deserialize(buf);
                                        Send(p);
                                        break;
                                    }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(NetworkSettings.NetworkNodeThreadSleep);
                    }
                        
                }
                catch (Exception)
                {
                    break;
                }
            }
            Log("Stopping Slave Network Node. Thead ID: " + Thread.CurrentThread.ManagedThreadId);
            m_Exit = false;
            m_Client.Close();
        }


    public void Connect(NetworkAdapter adapter)
        {
            lock (m_Client)
            {
                m_Client.GetStream().Write(new[] { (byte)NetworkNodeRequestType.CONNECT }, 0, 1);
                byte[] buf = new byte[sizeof(int)];
                int read = m_Client.GetStream().Read(buf, 0, buf.Length);
                int addr = BitConverter.ToInt32(buf, 0);
                adapter.NetworkAddress = addr;
                m_LocalAdapters.TryAdd(adapter.NetworkAddress, adapter);
            }
        }

        public void Disconnect(int address)
        {
            lock (m_Client)
            {
                List<byte> request = new List<byte>();
                request.Add((byte)NetworkNodeRequestType.DISCONNECT);
                request.AddRange(BitConverter.GetBytes(address));
                m_Client.GetStream().Write(request.ToArray(), 0, request.Count);
                m_LocalAdapters.TryRemove(address, out NetworkAdapter v);
                v.NetworkAddress = -1;
            }
        }


        public void Send(NetworkPacket p)
        {
            if(m_LocalAdapters.TryGetValue(p.Receiver.NetworkAddress, out NetworkAdapter localAdapter))
            {
                localAdapter.EnqueuePacket(p);
            }
            else
            {
                    List<byte> request = new List<byte>();
                    request.Add((byte)NetworkNodeRequestType.SEND);
                    byte[] data = p.Serialize();
                    request.AddRange(BitConverter.GetBytes(data.Length));
                    request.AddRange(data);

                lock (m_Client)
                {
                    m_Client.GetStream().Write(request.ToArray(), 0, request.Count);
                }
            }
        }
    }
}
