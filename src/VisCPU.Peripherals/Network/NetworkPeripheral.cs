﻿using System;
using System.Collections.Generic;
using System.Linq;

using VisCPU.Utility.IO.Settings;

namespace VisCPU.Peripherals.Network
{

    public class NetworkPeripheral : Peripheral
    {

        public enum Command
        {

            CONNECT,    //No Args
            DISCONNECT, //No Args
            OPEN_PORT,  //Port
            CLOSE_PORT, //Port

            SEND,            //SenderPort, Address, Port, Data, Length
            HAS_PACKET,      //Port
            GET_PACKET_SIZE, //Port
            GET_PACKET_DATA, //Port, RamAddress
            CONSUME_PACKET,  //Port 
            GET_SENDER_ADDR, //Port
            GET_SENDER_PORT, //Port
            GET_ADDR,        //No Args
            IS_CONNECTED,    //No Args,
            IS_OPEN_PORT,
            GET_GUID

        }

        private readonly Queue < uint > m_ReturnQueue = new Queue < uint >();
        private readonly Stack < uint > m_ArgumentStack = new Stack < uint >();

        private readonly NetworkAdapter m_Adapter;

        public override string PeripheralName => "Network Adapter";

        public override PeripheralType PeripheralType => PeripheralType.Custom;

        public override uint PresentPin => 0xFFFF8000;

        public override uint AddressRangeStart => PresentPin;

        public override uint AddressRangeEnd => m_ArgumentPin;

        private uint m_CommandPin => PresentPin + 1;

        private uint m_ArgumentPin => PresentPin + 2;

        #region Unity Event Functions

        public override void Reset()
        {
            m_Adapter.Reset();
        }

        #endregion

        #region Public

        public NetworkPeripheral()
        {
            NetworkSettings settings = SettingsManager.GetSettings < NetworkSettings >();
            m_Adapter = new NetworkAdapter( settings.GetNode() );
            m_Adapter.GUID = settings.NodeAdapterGUID;
        }
        

        public override uint ReadData( uint address )
        {
            if ( address == PresentPin )
            {
                return 1;
            }

            if ( address == m_CommandPin )
            {
                if ( m_ReturnQueue.Count == 0 )
                    AttachedCpu.FireInterrupt( 1 );
                return m_ReturnQueue.Dequeue();
            }

            return 0;
        }

        public override void Shutdown()
        {
            INetworkNode node = m_Adapter.NetworkNode;

            m_Adapter.Unload();
            node.UnloadNode();
            base.Shutdown();
        }

        public override void WriteData( uint address, uint data )
        {
            if ( address == m_ArgumentPin )
            {
                m_ArgumentStack.Push( data );
            }
            else if ( address == m_CommandPin )
            {
                Command cmd = ( Command ) data;

                switch ( cmd )
                {
                    case Command.CONNECT:
                        m_Adapter.Connect();

                        break;

                    case Command.DISCONNECT:
                        m_Adapter.Disconnect();

                        break;

                    case Command.OPEN_PORT:
                        m_Adapter.OpenPort( ( int ) m_ArgumentStack.Pop() );

                        break;

                    case Command.CLOSE_PORT:
                        m_Adapter.ClosePort( ( int ) m_ArgumentStack.Pop() );

                        break;

                    case Command.SEND:
                        SendCommand();

                        break;

                    case Command.HAS_PACKET:
                        uint ret = ( uint ) m_Adapter.GetListener( ( int ) m_ArgumentStack.Pop() ).PacketCount;
                        m_ReturnQueue.Enqueue( ret );

                        break;

                    case Command.GET_PACKET_SIZE:
                    {
                        NetworkPacket p = m_Adapter.GetListener( ( int ) m_ArgumentStack.Pop() ).PeekPacket();

                        m_ReturnQueue.Enqueue( ( uint ) p.Data.Length );

                        break;
                    }

                    case Command.GET_PACKET_DATA:
                        ReadCommand();

                        break;

                    case Command.CONSUME_PACKET:
                    {
                        m_Adapter.GetListener( ( int ) m_ArgumentStack.Pop() ).ReadPacket();

                        break;
                    }

                    case Command.GET_SENDER_ADDR:
                    {
                        NetworkPacket p = m_Adapter.GetListener( ( int ) m_ArgumentStack.Pop() ).PeekPacket();
                        m_ReturnQueue.Enqueue( ( uint ) p.Sender.NetworkAddress );

                        break;
                    }

                    case Command.GET_SENDER_PORT:
                    {
                        NetworkPacket p = m_Adapter.GetListener( ( int ) m_ArgumentStack.Pop() ).PeekPacket();
                        m_ReturnQueue.Enqueue( ( uint ) p.Sender.Port );

                        break;
                    }

                    case Command.GET_ADDR:
                        m_ReturnQueue.Enqueue( ( uint ) m_Adapter.NetworkAddress );

                        break;

                    case Command.IS_CONNECTED:
                        m_ReturnQueue.Enqueue( m_Adapter.IsConnected ? 1u : 0u );

                        break;

                    case Command.IS_OPEN_PORT:
                        m_ReturnQueue.Enqueue( m_Adapter.IsPortOpen( ( int ) m_ArgumentStack.Pop() ) ? 1u : 0u );

                        break;

                    case Command.GET_GUID:
                        GetGuid();

                        break;
                }
            }
        }

        #endregion

        #region Private

        private void GetGuid()
        {
            uint addr = m_ArgumentStack.Pop();

            uint[] cpuRam = AttachedCpu.MemoryBus.GetPeripherals < Memory.Memory >().First().GetInternalBuffer();

            uint[] data = m_Adapter.GUID.Select( x => ( uint ) x ).ToArray();

            Array.Copy( data, 0, cpuRam, addr, data.Length );
        }

        private void ReadCommand()
        {
            uint addr = m_ArgumentStack.Pop();
            int port = ( int ) m_ArgumentStack.Pop();

            if ( m_Adapter.GetListener( port ).TryPeekPacket( out NetworkPacket packet ) )
            {
                uint[] cpuRam = AttachedCpu.MemoryBus.GetPeripherals < Memory.Memory >().First().GetInternalBuffer();

                Array.Copy( packet.Data, 0, cpuRam, addr, packet.Data.Length );
            }
        }

        private void SendCommand()
        {
            int dataLength = ( int ) m_ArgumentStack.Pop();
            int data = ( int ) m_ArgumentStack.Pop();
            int receiverPort = ( int ) m_ArgumentStack.Pop();
            int receiverAddress = ( int ) m_ArgumentStack.Pop();
            int senderPort = ( int ) m_ArgumentStack.Pop();

            uint[] cpuRam = AttachedCpu.MemoryBus.GetPeripherals < Memory.Memory >().First().GetInternalBuffer();
            uint[] dataBuf = new uint[dataLength];

            Array.Copy( cpuRam, data, dataBuf, 0, dataLength );

            m_Adapter.NetworkNode.Send(
                                       m_Adapter.CreateIdentity( senderPort ).
                                                 CreatePacket(
                                                              Network.CreateIdentity( receiverAddress, receiverPort ),
                                                              dataBuf
                                                             )
                                      );
        }

        #endregion

    }

}
