using System;
using System.Text;
using VisCPU.Peripherals;

namespace VisCPU.Tests.Utils
{

    public class TestDevice : Peripheral
    {
        private const uint TestBegin = 0xFFFF2000;
        private const uint TestFail = 0xFFFF2001;
        private const uint TestPass = 0xFFFF2002;
        private const uint TestDevicePresent = 0xFFFF2003;
        private readonly StringBuilder m_TextBuilder = new StringBuilder();
        private uint m_CurrentCommand;

        private string m_CurrentTest;

        public event Action < string > OnPass;

        public event Action < string, string > OnFail;

        #region Unity Event Functions

        public override void Reset()
        {
            m_CurrentCommand = 0;
            m_CurrentTest = null;
        }

        #endregion

        #region Public

        public override bool CanRead( uint address )
        {
            return address == TestDevicePresent;
        }

        public override bool CanWrite( uint address )
        {
            return address == TestBegin || address == TestFail || address == TestPass;
        }

        public override uint ReadData( uint address )
        {
            if ( address == TestDevicePresent )
            {
                return 1;
            }

            throw new NotImplementedException();
        }

        public override void WriteData( uint address, uint data )
        {
            if ( m_CurrentCommand != 0 && m_CurrentCommand != address )
            {
                throw new TestDeviceException( $"Finish Command {m_CurrentCommand} before starting {address} command" );
            }

            if ( address == TestPass && data != 0 )
            {
                PassTest();

                return;
            }

            m_CurrentCommand = address;

            bool runCommand = false;

            if ( m_CurrentCommand != 0 )
            {
                if ( data != 0 )
                {
                    m_TextBuilder.Append( ( char ) data );
                }
                else
                {
                    runCommand = true;
                }
            }

            if ( runCommand )
            {
                switch ( address )
                {
                    case TestBegin:
                        BeginTest();

                        break;

                    case TestFail:
                        FailTest();

                        break;

                    default:
                        throw new NotSupportedException( "Invalid Test Device Command" );
                }

                m_CurrentCommand = 0;
                m_TextBuilder.Clear();
            }
        }

        #endregion

        #region Private

        private void BeginTest()
        {
            if ( m_CurrentTest != null )
            {
                throw new TestDeviceException( $"Finish test {m_CurrentTest} before starting test {m_TextBuilder}" );
            }

            m_CurrentTest = m_TextBuilder.ToString();
        }

        private void FailTest()
        {
            string testName = m_CurrentTest;
            string desc = m_TextBuilder.ToString();
            m_CurrentTest = null;
            m_CurrentCommand = 0;
            m_TextBuilder.Clear();
            OnFail?.Invoke( testName, desc );
        }

        private void PassTest()
        {
            OnPass?.Invoke( m_CurrentTest );
            m_CurrentTest = null;
        }

        #endregion
    }

}
