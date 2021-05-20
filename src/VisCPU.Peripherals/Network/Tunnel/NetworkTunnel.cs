using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace VisCPU.Peripherals.Network
{

    public class NetworkTunnel : NetworkBase
    {

        private readonly TcpClient m_Client;
        private readonly LocalNetworkNode m_Node;
        private Thread m_Thread;
        private bool m_Exit;

        private readonly ConcurrentDictionary < int, object > m_RemoteAdapters =
            new ConcurrentDictionary < int, object >();

        public bool IsRunning => m_Thread != null && m_Thread.IsAlive;

        public EndPoint Endpoint => m_Client.Client.RemoteEndPoint;

        #region Public

        public NetworkTunnel( LocalNetworkNode node, TcpClient client )
        {
            m_Node = node;
            m_Client = client;
            m_Node.OnPacketDrop += M_Node_OnPacketDrop;
        }

        public int[] GetRemoteAdapters()
        {
            return m_RemoteAdapters.Keys.ToArray();
        }

        public void StartTunnel()
        {
            StopTunnel();
            m_Thread = new Thread( Loop );
            m_Thread.Start();
        }

        public void StopTunnel()
        {
            if ( m_Thread == null || !m_Thread.IsAlive )
            {
                return;
            }

            m_Exit = true;
        }

        #endregion

        #region Private

        private void Loop()
        {
            Log( "Starting Network Tunnel. Thead ID: " + Thread.CurrentThread.ManagedThreadId );

            while ( true )
            {
                try
                {
                    if ( m_Exit || !m_Client.Connected )
                    {
                        break;
                    }

                    if ( m_Client.Available >= 1 )
                    {
                        NetworkNodeRequestType type = ( NetworkNodeRequestType ) m_Client.GetStream().ReadByte();
                        byte[] buf;

                        switch ( type )
                        {
                            case NetworkNodeRequestType.DISCONNECT:
                            {
                                buf = new byte[sizeof( int )];
                                m_Client.GetStream().Read( buf, 0, buf.Length );
                                int addr = BitConverter.ToInt32( buf, 0 );
                                m_Node.Disconnect( addr );
                                m_RemoteAdapters.TryRemove( addr, out object a );

                                break;
                            }

                            case NetworkNodeRequestType.CONNECT:
                            {
                                int addr = m_Node.RegisterAddress();
                                m_RemoteAdapters.TryAdd( addr, null );

                                buf = BitConverter.GetBytes( addr );
                                m_Client.GetStream().Write( buf, 0, buf.Length );

                                break;
                            }

                            case NetworkNodeRequestType.SEND:
                            {
                                buf = new byte[sizeof( int )];
                                m_Client.GetStream().Read( buf, 0, buf.Length );
                                int len = BitConverter.ToInt32( buf, 0 );
                                buf = new byte[len];
                                m_Client.GetStream().Read( buf, 0, buf.Length );
                                m_Node.Send( NetworkPacket.Deserialize( buf ) );

                                break;
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep( NetworkSettings.NetworkTunnelThreadSleep );
                    }
                }
                catch ( Exception )
                {
                    break;
                }
            }

            Log( "Stopping Network Tunnel. Thead ID: " + Thread.CurrentThread.ManagedThreadId );
            m_Exit = false;
            m_Client.Close();
        }

        private void M_Node_OnPacketDrop( NetworkPacket packet )
        {
            if ( m_RemoteAdapters.ContainsKey( packet.Receiver.NetworkAddress ) )
            {
                Log( "Routing Packet through tunnel '{0}'", m_Client.Client.RemoteEndPoint );
                List < byte > request = new List < byte >();
                request.Add( ( byte ) NetworkNodeRequestType.SEND );
                byte[] data = packet.Serialize();
                request.AddRange( BitConverter.GetBytes( data.Length ) );
                request.AddRange( data );
                m_Client.GetStream().Write( request.ToArray(), 0, request.Count );
            }
        }

        #endregion

    }

}
