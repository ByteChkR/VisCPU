namespace VisCPU.Peripherals.Network
{

    public interface INetworkNode
    {

        void Connect( NetworkAdapter adapter );

        void Disconnect( int adapterAddress );

        void Send( NetworkPacket p );

        void UnloadNode();

    }

}
