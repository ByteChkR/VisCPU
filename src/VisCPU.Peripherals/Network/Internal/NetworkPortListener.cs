using System.Collections.Concurrent;
using VisCPU.Utility.Logging;

namespace VisCPU.Peripherals.Network
{
    public class NetworkPortListener
    {
        private readonly NetworkIdentity m_Identity;
        public NetworkIdentity GetIdentity() => m_Identity;
        public bool IsOpen { get; private set; }
        public NetworkPortListener(NetworkIdentity id)
        {
            m_Identity = id;
        }
        private ConcurrentQueue<NetworkPacket> m_PacketQueue = new ConcurrentQueue<NetworkPacket>();
        public int PacketCount => m_PacketQueue.Count;

        internal void EnqueuePacket(NetworkPacket packet)
        {
            if (IsOpen && packet.Receiver.NetworkAddress == m_Identity.NetworkAddress && packet.Receiver.Port == m_Identity.Port)
                m_PacketQueue.Enqueue(packet);
        }

        public bool TryReadPacket(out NetworkPacket packet) => m_PacketQueue.TryDequeue(out packet);
        public NetworkPacket ReadPacket()
        {
            if (TryReadPacket(out NetworkPacket p))
            {
                return p;
            }
            throw new System.Exception("Can not Read the Packet.");
        }
        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
            m_PacketQueue = new ConcurrentQueue<NetworkPacket>();
        }

        public bool TryPeekPacket(out NetworkPacket packet) => m_PacketQueue.TryPeek(out packet);
        public NetworkPacket PeekPacket()
        {
            if(TryPeekPacket(out NetworkPacket p))
            {
                return p;
            }
            throw new System.Exception("Can not Peek the Packet.");
        }
    }
}
