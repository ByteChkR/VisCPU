using System;
using System.Collections.Concurrent;

namespace VisCPU.Peripherals.Network
{

    public class NetworkPortListener
    {

        private readonly NetworkIdentity m_Identity;
        private ConcurrentQueue < NetworkPacket > m_PacketQueue = new ConcurrentQueue < NetworkPacket >();

        public bool IsOpen { get; private set; }

        public int PacketCount => m_PacketQueue.Count;

        #region Public

        public NetworkPortListener( NetworkIdentity id )
        {
            m_Identity = id;
        }

        public void Close()
        {
            IsOpen = false;
            m_PacketQueue = new ConcurrentQueue < NetworkPacket >();
        }

        public NetworkIdentity GetIdentity()
        {
            return m_Identity;
        }

        public void Open()
        {
            IsOpen = true;
        }

        public NetworkPacket PeekPacket()
        {
            if ( TryPeekPacket( out NetworkPacket p ) )
            {
                return p;
            }

            throw new Exception( "Can not Peek the Packet." );
        }

        public NetworkPacket ReadPacket()
        {
            if ( TryReadPacket( out NetworkPacket p ) )
            {
                return p;
            }

            throw new Exception( "Can not Read the Packet." );
        }

        public bool TryPeekPacket( out NetworkPacket packet )
        {
            return m_PacketQueue.TryPeek( out packet );
        }

        public bool TryReadPacket( out NetworkPacket packet )
        {
            return m_PacketQueue.TryDequeue( out packet );
        }

        internal void EnqueuePacket( NetworkPacket packet )
        {
            if ( IsOpen &&
                 packet.Receiver.NetworkAddress == m_Identity.NetworkAddress &&
                 packet.Receiver.Port == m_Identity.Port )
            {
                m_PacketQueue.Enqueue( packet );
            }
        }

        #endregion

    }

}
