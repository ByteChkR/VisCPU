namespace VisCPU.Peripherals.Network
{

    public static class Network
    {
        public static NetworkIdentity CreateIdentity(int networkAddress, int port) => new NetworkIdentity { NetworkAddress = networkAddress, Port = port };
        public static NetworkIdentity CreateIdentity(this NetworkAdapter adapter, int port) => CreateIdentity(adapter.NetworkAddress, port);
        public static NetworkPacket CreatePacket(this NetworkIdentity id, NetworkIdentity receiver, uint[] data = null) => new NetworkPacket { Sender = id, Receiver = receiver, Data = data };

    }
}
