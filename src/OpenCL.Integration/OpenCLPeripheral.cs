using System.Collections.Generic;
using System.Linq;

using OpenCL.NET;
using OpenCL.NET.CommandQueues;
using OpenCL.NET.Contexts;
using OpenCL.NET.Devices;
using OpenCL.NET.Platforms;
using OpenCL.Wrapper;
using OpenCL.Wrapper.TypeEnums;

using VisCPU.Peripherals;

namespace OpenCL.Integration
{

    public class OpenCLPeripheral : Peripheral
    {

        private KernelDatabase m_Database = new KernelDatabase( DataVectorTypes.Uchar1 );

        private uint m_PlatformIdxSelector;
        private uint m_DeviceCountPlatformIdxSelector;
        private uint m_DevicePlatformIdxSelector;
        private uint m_CreateContextDeviceIdxSelector;
        private uint m_CreateCommandQueueContextStep;
        private uint m_CreateCommandQueueContextIdxSelector;
        private uint m_CreateCommandQueueDeviceIdxSelector;

        private readonly List < HandleBase > m_Handles = new List < HandleBase >();

        public override string PeripheralName => "OpenCL Device";

        public override PeripheralType PeripheralType => PeripheralType.Custom;

        public override uint PresentPin => 0xFFFF7000;

        public uint GetPlatformCountPin => 0xFFFF7001;

        public uint GetPlatformPin => 0xFFFF7002;

        public uint GetDeviceCountPin => 0xFFFF7003;

        public uint GetDevicePin => 0xFFFF7004;

        public uint CreateContextPin => 0xFFFF7005;

        public uint CreateCommandQueuePin => 0xFFFF7006;

        #region Unity Event Functions

        public override void Reset()
        {
            foreach ( HandleBase handleBase in m_Handles )
            {
                handleBase.Dispose();
            }

            m_Handles.Clear();
        }

        #endregion

        #region Public

        public override bool CanRead( uint address )
        {
            return PresentPin == address ||
                   GetPlatformCountPin == address ||
                   GetPlatformPin == address ||
                   GetDeviceCountPin == address ||
                   GetDevicePin == address ||
                   CreateContextPin == address ||
                   CreateCommandQueuePin == address;
        }

        public override bool CanWrite( uint address )
        {
            return GetPlatformPin == address ||
                   GetDeviceCountPin == address ||
                   GetDevicePin == address ||
                   CreateContextPin == address ||
                   CreateCommandQueuePin == address;
        }

        public override uint ReadData( uint address )
        {
            if ( address == PresentPin )
            {
                return 1;
            }

            if ( address == GetPlatformCountPin )
            {
                return Platform.GetPlatformCount();
            }

            if ( address == GetDeviceCountPin )
            {
                return ( m_Handles[( int ) m_DeviceCountPlatformIdxSelector] as Platform ).GetDeviceCount(
                     DeviceType.All
                    );
            }

            if ( address == GetDevicePin )
            {
                Device p = ( m_Handles[( int ) m_DevicePlatformIdxSelector] as Platform ).GetDevices( DeviceType.All ).
                    Skip( ( int ) m_PlatformIdxSelector ).
                    First();

                ;
                m_Handles.Add( p );

                return ( uint ) m_Handles.Count - 1;
            }

            if ( address == GetPlatformPin )
            {
                Platform p = Platform.GetPlatforms().Skip( ( int ) m_PlatformIdxSelector ).First();
                m_Handles.Add( p );

                return ( uint ) m_Handles.Count - 1;
            }

            if ( address == CreateContextPin )
            {
                Context p = Context.CreateContext( m_Handles[( int ) m_CreateContextDeviceIdxSelector] as Device );
                m_Handles.Add( p );

                return ( uint ) m_Handles.Count - 1;
            }

            if ( address == CreateCommandQueuePin )
            {
                m_CreateCommandQueueContextStep = 0;
                Device d = m_Handles[( int ) m_CreateCommandQueueDeviceIdxSelector] as Device;
                Context c = m_Handles[( int ) m_CreateCommandQueueContextIdxSelector] as Context;
                CommandQueue q = CommandQueue.CreateCommandQueue( c, d );
                m_Handles.Add( q );

                return ( uint ) m_Handles.Count - 1;
            }

            return 0;
        }

        public override void WriteData( uint address, uint data )
        {
            if ( address == GetDeviceCountPin )
            {
                m_DeviceCountPlatformIdxSelector = data;
            }

            if ( address == GetPlatformPin )
            {
                m_PlatformIdxSelector = data;
            }

            if ( address == GetDevicePin )
            {
                m_DevicePlatformIdxSelector = data;
            }

            if ( address == CreateContextPin )
            {
                m_CreateContextDeviceIdxSelector = data;
            }

            if ( address == CreateCommandQueuePin )
            {
                if ( m_CreateCommandQueueContextStep == 0 )
                {
                    m_CreateCommandQueueContextStep++;
                    m_CreateCommandQueueContextIdxSelector = data;
                }
                else if ( m_CreateCommandQueueContextStep == 1 )
                {
                    m_CreateCommandQueueDeviceIdxSelector = data;
                }
            }
        }

        #endregion

    }

}
