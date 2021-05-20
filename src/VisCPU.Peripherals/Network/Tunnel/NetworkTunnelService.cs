using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace VisCPU.Peripherals.Network
{

    public class NetworkTunnelService : NetworkBase
    {

        private readonly TcpListener m_Listener;
        private Thread m_Thread;
        private bool m_Exit;
        private LocalNetworkNode m_Node;
        private readonly List < NetworkTunnel > m_Tunnels = new List < NetworkTunnel >();

        #region Unity Event Functions

        public void Start()
        {
            Stop();
            m_Thread = new Thread( Loop );
            m_Thread.Start();
        }

        #endregion

        #region Public

        public NetworkTunnelService( LocalNetworkNode node, int port )
        {
            m_Node = node;
            m_Listener = new TcpListener( IPAddress.Any, port );
        }

        public NetworkTunnel[] GetActiveNetworkTunnels()
        {
            return m_Tunnels.ToArray();
        }

        public void Stop()
        {
            if ( m_Thread == null || !m_Thread.IsAlive )
            {
                return;
            }

            Log( "Stopping Network Tunnel Service. Thead ID: " + m_Thread.ManagedThreadId );
            m_Exit = true;

            while ( m_Exit )
            {
            } //Wait for Thread to Exit
        }

        #endregion

        #region Private

        private void Loop()
        {
            Log( "Starting Network Tunnel Service. Thead ID: " + Thread.CurrentThread.ManagedThreadId );
            m_Listener.Start();

            while ( true )
            {
                if ( m_Exit )
                {
                    break;
                }

                if ( m_Listener.Pending() )
                {
                    NetworkTunnel t = new NetworkTunnel( m_Node, m_Listener.AcceptTcpClient() );
                    m_Tunnels.Add( t );
                    t.StartTunnel();
                }
                else
                {
                    Thread.Sleep( NetworkSettings.NetworkTunnelServiceThreadSleep );
                }
            }

            m_Listener.Stop();
            m_Exit = false;

            foreach ( NetworkTunnel tunnel in m_Tunnels )
            {
                tunnel.StopTunnel();
            }
        }

        #endregion

    }

}
