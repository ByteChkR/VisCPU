using System.Text;
using System.Threading;

namespace VisCPU.Peripherals.Network.Internal
{

    public class EchoNetworkAdapter : NetworkAdapter
    {

        private const int ECHO_SERVER_PORT = 444;
        private bool m_Exit;
        private Thread m_Thread;

        #region Public

        public EchoNetworkAdapter( INetworkNode node ) : base( node )
        {
            OnConnect += EchoNetworkAdapter_OnConnect;
            OnDisconnect += EchoNetworkAdapter_OnDisconnect;
        }

        #endregion

        #region Private

        private void EchoNetworkAdapter_OnConnect()
        {
            EchoNetworkAdapter_OnDisconnect();
            m_Thread = new Thread( Loop );
            m_Thread.Start();
        }

        private void EchoNetworkAdapter_OnDisconnect()
        {
            if ( m_Thread == null || !m_Thread.IsAlive )
            {
                return;
            }

            m_Exit = true;

            while ( m_Exit )
            {
            }
        }

        private void Loop()
        {
            Log( "Starting Echo Server. Thead ID: " + Thread.CurrentThread.ManagedThreadId );
            NetworkPortListener listener = OpenPort( ECHO_SERVER_PORT );

            while ( true )
            {
                if ( m_Exit )
                {
                    break;
                }

                while ( listener.PacketCount != 0 )
                {
                    NetworkPacket packet = listener.ReadPacket();

                    packet.Respond( this, packet.Data ); //Echo the Packet

                    StringBuilder sb = new StringBuilder();

                    foreach ( uint data in packet.Data )
                    {
                        sb.Append( ( char ) data );
                    }

                    Log( "Echo Package Contents: {0}", sb );
                }

                Thread.Sleep( NetworkSettings.EchoServerThreadSleep );
            }

            m_Exit = false;
        }

        #endregion

    }

}