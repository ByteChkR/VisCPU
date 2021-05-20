namespace VisCPU.Peripherals.Network
{

    public static class Network
    {

        #region Public

        public static NetworkIdentity CreateIdentity( int networkAddress, int port )
        {
            return new NetworkIdentity
                   {
                       NetworkAddress = networkAddress,
                       Port = port
                   };
        }

        public static NetworkIdentity CreateIdentity( this NetworkAdapter adapter, int port )
        {
            return CreateIdentity( adapter.NetworkAddress, port );
        }

        public static NetworkPacket CreatePacket(
            this NetworkIdentity id,
            NetworkIdentity receiver,
            uint[] data = null )
        {
            return new NetworkPacket
                   {
                       Sender = id,
                       Receiver = receiver,
                       Data = data
                   };
        }

        #endregion

    }

}
