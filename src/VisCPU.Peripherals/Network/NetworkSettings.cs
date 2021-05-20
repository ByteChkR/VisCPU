using System;

using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Peripherals.Network
{

    public class NetworkSettings
    {

        public static int NetworkNodeThreadSleep = 100;
        public static int NetworkTunnelServiceThreadSleep = 100;
        public static int NetworkTunnelThreadSleep = 100;
        public static int EchoServerThreadSleep = 100;
        public static int DNSServerThreadSleep = 100;
        public string NodeType = "LOCAL";
        public string NodeSlaveHost = "localhost";
        public int NodeSlavePort = 42069;
        public string NodeAdapterGUID = Guid.NewGuid().ToString();

        #region Public

        public INetworkNode GetNode()
        {
            if ( NodeType == "LOCAL" )
            {
                return new LocalNetworkNode();
            }
            else if ( NodeType == "SLAVE" )
            {
                return new SlaveNetworkNode( NodeSlaveHost, NodeSlavePort );
            }

            throw new ArgumentException( "Invalid Node Type: " + NodeType );
        }

        #endregion

        #region Private

        static NetworkSettings()
        {
            SettingsCategory coutCategory = Peripheral.PeripheralCategory.AddCategory( "network" );

            SettingsManager.RegisterDefaultLoader(
                                                  new JsonSettingsLoader(),
                                                  coutCategory,
                                                  "node.json",
                                                  new NetworkSettings()
                                                 );
        }

        #endregion

    }

}
