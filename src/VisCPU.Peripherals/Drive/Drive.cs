namespace VisCPU.Peripherals.Drive
{

    public abstract class Drive : Peripheral
    {
        protected DrivePinSettings m_Settings;

        private uint m_WriteDataStep = 0;
        private uint m_WriteDataAddr = 0;

        private uint m_ReadDataStep = 0;
        private uint m_ReadDataAddr = 0;

        private uint m_WriteBufferStep = 0;
        private uint m_WriteBufferDstAddr = 0;
        private uint m_WriteBufferSrcAddr = 0;
        private uint m_WriteBufferSrcStart = 0;
        private uint m_WriteBufferSrcLen = 0;

        private uint m_ReadBufferStep = 0;
        private uint m_ReadBufferDstAddr = 0;
        private uint m_ReadBufferSrcAddr = 0;
        private uint m_ReadBufferSrcStart = 0;
        private uint m_ReadBufferSrcLen = 0;

        public override PeripheralType PeripheralType => PeripheralType.Drive;

        public override uint PresentPin => m_Settings.PresentAddress;

        #region Public

        public abstract uint GetSize();

        public abstract uint Read( uint address );

        public abstract void ReadBuffer( uint address, uint destination, uint start, uint length );

        public abstract void Write( uint address, uint data );

        public abstract void WriteBuffer( uint address, uint destination, uint start, uint length );

        public override bool CanRead( uint address )
        {
            return m_Settings.GetSizeAddress == address ||
                   m_Settings.PresentAddress == address ||
                   m_Settings.ReadAddress == address;
        }

        public override bool CanWrite( uint address )
        {
            return m_Settings.GetSizeAddress == address ||
                   m_Settings.ReadAddress == address ||
                   m_Settings.ReadBufferAddress == address ||
                   m_Settings.WriteAddress == address ||
                   m_Settings.WriteBufferAddress == address;
        }

        public override uint ReadData( uint address )
        {
            if ( m_Settings.PresentAddress == address )
            {
                return 1;
            }

            if ( m_Settings.ReadAddress == address && m_ReadDataStep == 1 )
            {
                m_ReadDataStep = 0;

                return Read( m_ReadDataAddr );
            }

            if ( m_Settings.GetSizeAddress == address )
            {
                return GetSize();
            }

            return 0;
        }

        public override void WriteData( uint address, uint data )
        {
            if ( address == m_Settings.WriteAddress )
            {
                if ( m_WriteDataStep == 0 )
                {
                    m_WriteDataStep++;
                    m_WriteDataAddr = data;
                }
                else
                {
                    m_WriteDataStep = 0;
                    Write( m_WriteDataAddr, data );
                }
            }

            if ( address == m_Settings.ReadAddress )
            {
                if ( m_ReadDataStep == 0 )
                {
                    m_ReadDataStep++;
                    m_ReadDataAddr = data;
                }
            }

            if ( address == m_Settings.WriteBufferAddress )
            {
                if ( m_WriteBufferStep == 0 )
                {
                    m_WriteBufferStep++;
                    m_WriteBufferDstAddr = data;
                }
                else if ( m_WriteBufferStep == 1 )
                {
                    m_WriteBufferStep++;
                    m_WriteBufferSrcAddr = data;
                }
                else if ( m_WriteBufferStep == 2 )
                {
                    m_WriteBufferStep++;
                    m_WriteBufferSrcStart = data;
                }
                else
                {
                    m_WriteBufferSrcLen = data;
                    m_WriteBufferStep = 0;

                    WriteBuffer(
                        m_WriteBufferDstAddr,
                        m_WriteBufferSrcAddr,
                        m_WriteBufferSrcStart,
                        m_WriteBufferSrcLen
                    );
                }

            }

            if ( address == m_Settings.ReadBufferAddress )
            {
                if ( m_ReadBufferStep == 0 )
                {
                    m_ReadBufferStep++;
                    m_ReadBufferDstAddr = data;
                }
                else if ( m_ReadBufferStep == 1 )
                {
                    m_ReadBufferStep++;
                    m_ReadBufferSrcAddr = data;
                }
                else if ( m_ReadBufferStep == 2 )
                {
                    m_ReadBufferStep++;
                    m_ReadBufferSrcStart = data;
                }
                else
                {
                    m_ReadBufferSrcLen = data;
                    m_ReadBufferStep = 0;

                    ReadBuffer( m_ReadBufferDstAddr, m_ReadBufferSrcAddr, m_ReadBufferSrcStart, m_ReadBufferSrcLen );
                }
            }
        }

        #endregion

        #region Protected

        protected Drive( DrivePinSettings settings )
        {
            m_Settings = settings;

        }

        #endregion
    }

}