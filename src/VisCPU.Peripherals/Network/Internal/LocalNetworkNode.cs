using System;
using System.Collections.Concurrent;
using System.Linq;

using VisCPU.Peripherals.Network.Internal;

namespace VisCPU.Peripherals.Network
{

    public class LocalNetworkNode : NetworkBase, INetworkNode
    {

        private int s_NextAddress = 1;

        private readonly ConcurrentDictionary < int, NetworkAdapter > s_NetworkAdapters =
            new ConcurrentDictionary < int, NetworkAdapter >();

        public event Action < NetworkPacket > OnPacketDrop;

        public DNSNetworkAdapter DNSAdapter { get; }

        public EchoNetworkAdapter EchoAdapter { get; }

        #region Public

        public LocalNetworkNode( params NetworkAdapter[] adapters )
        {
            //DNS Adapter Name: "dns.server.com"
            //Create DNS Service
            DNSAdapter = new DNSNetworkAdapter( this, "dns.server.com" );
            DNSAdapter.Connect();

            //Echo Adapter Name: "echo.server.com"
            //Create Echo Service
            EchoAdapter = new EchoNetworkAdapter( this );
            EchoAdapter.Connect();

            //Register as echo.server.com
            DNSAdapter.Register( "echo.server.com", EchoAdapter.GUID, EchoAdapter.NetworkAddress );
        }

        public void Connect( NetworkAdapter adapter )
        {
            adapter.NetworkAddress = RegisterAddress();

            if ( !s_NetworkAdapters.TryAdd( adapter.NetworkAddress, adapter ) )
            {
                throw new Exception( "Can not Add Network Adapter" );
            }
        }

        public void Disconnect( NetworkAdapter adapter )
        {
            Disconnect( adapter.NetworkAddress );
        }

        public void Disconnect( int adapterAddress )
        {
            if ( !s_NetworkAdapters.TryRemove( adapterAddress, out NetworkAdapter v ) )
            {
                Log( "Can not Remove Network Adapter: {0}", adapterAddress );
            }
            else
            {
                v.NetworkAddress = -1;
            }
        }

        public NetworkAdapter[] GetAllAdapters()
        {
            return s_NetworkAdapters.Values.ToArray();
        }

        public int RegisterAddress()
        {
            int v = s_NextAddress;
            s_NextAddress++;

            return v;
        }

        public void Send( NetworkPacket p )
        {
            if ( s_NetworkAdapters.TryGetValue( p.Receiver.NetworkAddress, out NetworkAdapter adapter ) )
            {
                adapter.EnqueuePacket( p );
            }
            else
            {
                OnPacketDrop?.Invoke( p );
            }
        }

        public void UnloadNode()
        {
            //Empty. No Threads to Suspend/Exit
            foreach ( NetworkAdapter adapter in s_NetworkAdapters.Values.ToArray() )
            {
                adapter.Unload();
            }
        }

        #endregion

    }

}
