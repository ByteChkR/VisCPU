using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace VisCPU.Peripherals.Network
{

    public class NetworkAdapter : NetworkBase
    {

        private readonly ConcurrentDictionary < int, NetworkPortListener > m_OpenPorts =
            new ConcurrentDictionary < int, NetworkPortListener >();

        public event Action OnDisconnect;

        public event Action OnConnect;

        public string GUID { get; internal set; }

        public INetworkNode NetworkNode { get; internal set; }

        public int NetworkAddress { get; internal set; } = -1;

        public bool IsConnected => NetworkAddress != -1;

        #region Unity Event Functions

        public void Reset()
        {
            foreach ( KeyValuePair < int, NetworkPortListener > kvp in m_OpenPorts )
            {
                kvp.Value.Close();
            }

            Disconnect();
        }

        #endregion

        #region Public

        public NetworkAdapter( INetworkNode node )
        {
            NetworkNode = node;
            GUID = Guid.NewGuid().ToString(); //Create new
        }

        public void ClosePort( int port )
        {
            m_OpenPorts[port].Close();
        }

        public void Connect()
        {
            NetworkNode.Connect( this );
            OnConnect?.Invoke();
        }

        public void Disconnect()
        {
            OnDisconnect?.Invoke();
            NetworkNode.Disconnect( NetworkAddress );
        }

        public NetworkPortListener[] GetAllPortListeners()
        {
            return m_OpenPorts.Values.ToArray();
        }

        public NetworkPortListener GetListener( int port )
        {
            if ( !m_OpenPorts.TryGetValue( port, out NetworkPortListener listener ) || !listener.IsOpen )
            {
                throw new Exception( "Port is not Open" );
            }

            return listener;
        }

        public bool IsPortOpen( int port )
        {
            return m_OpenPorts.TryGetValue( port, out NetworkPortListener l ) && l.IsOpen;
        }

        public NetworkPortListener OpenPort( int port )
        {
            if ( m_OpenPorts.TryGetValue( port, out NetworkPortListener listener ) )
            {
                if ( listener.IsOpen )
                {
                    throw new Exception( "Port already Open" );
                }

                listener.Open();
            }

            listener = m_OpenPorts[port] = new NetworkPortListener( this.CreateIdentity( port ) );
            listener.Open();

            return listener;
        }

        public virtual void Unload()
        {
            //Empty. Nothing to Unload.
            Disconnect();
        }

        internal void EnqueuePacket( NetworkPacket packet )
        {
            if ( m_OpenPorts.TryGetValue( packet.Receiver.Port, out NetworkPortListener listener ) )
            {
                listener.EnqueuePacket( packet );
            }
        }

        #endregion

    }

}
