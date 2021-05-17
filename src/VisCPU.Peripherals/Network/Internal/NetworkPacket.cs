using System.Collections.Generic;
using System;
using System.Linq;
namespace VisCPU.Peripherals.Network
{
    public struct NetworkPacket
    {
        public NetworkIdentity Receiver { get; set; }
        public NetworkIdentity Sender { get; set; }
        private uint[] m_Data;
        public uint[] Data
        {
            get
            {
                return m_Data ?? new uint[0];
            }
            set
            {
                m_Data = value ?? new uint[0];
            }
        }

        public void Respond(NetworkAdapter ad, uint[] data) => ad.NetworkNode.Send(Receiver.CreatePacket(Sender, data));

        public byte[] Serialize()
        {
            
            List<byte> ret = new List<byte>();
            ret.AddRange(BitConverter.GetBytes(Receiver.NetworkAddress));
            ret.AddRange(BitConverter.GetBytes(Receiver.Port));
            ret.AddRange(BitConverter.GetBytes(Sender.NetworkAddress));
            ret.AddRange(BitConverter.GetBytes(Sender.Port));
            ret.AddRange(Data.SelectMany(x => BitConverter.GetBytes(x)));
            return ret.ToArray();
        }

        public static NetworkPacket Deserialize(byte[] data)
        {
            int recA = BitConverter.ToInt32(data, 0);
            int recP = BitConverter.ToInt32(data, sizeof(int));
            int senA = BitConverter.ToInt32(data, sizeof(int) * 2);
            int senP = BitConverter.ToInt32(data, sizeof(int) * 3);
            uint[] payload = new uint[(data.Length / sizeof(uint)) - 4];
            for(int i = 0; i < payload.Length; i++)
            {
                payload[i] = BitConverter.ToUInt32(data, 4 * sizeof(int) + i * sizeof(uint));
            }
            return Network.CreatePacket(
                    Network.CreateIdentity(senA, senP),
                    Network.CreateIdentity(recA, recP),
                    payload
                );
        }
    }
}
