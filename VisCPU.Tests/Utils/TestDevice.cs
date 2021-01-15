using System;
using System.Text;

namespace VisCPU.Tests.Utils
{

    public class TestDevice : Peripheral
    {

        private const uint TEST_BEGIN = 0xFFFF2000;
        private const uint TEST_FAIL = 0xFFFF2001;
        private const uint TEST_PASS = 0xFFFF2002;
        private const uint TEST_DEVICE_PRESENT = 0xFFFF2003;
        private readonly StringBuilder m_TextBuilder = new StringBuilder();
        private uint m_CurrentCommand;

        private string m_CurrentTest;

        public event Action < string > OnPass;

        public event Action < string, string > OnFail;

        #region Public

        public override bool CanRead( uint address )
        {
            return address == TEST_DEVICE_PRESENT;
        }

        public override bool CanWrite( uint address )
        {
            return address == TEST_BEGIN || address == TEST_FAIL || address == TEST_PASS;
        }

        public override uint ReadData( uint address )
        {
            if ( address == TEST_DEVICE_PRESENT )
            {
                return 1;
            }

            throw new NotImplementedException();
        }

        public override void WriteData( uint address, uint data )
        {
            if ( m_CurrentCommand != 0 && m_CurrentCommand != address )
            {
                throw new Exception( $"Finish Command {m_CurrentCommand} before starting {address} command" );
            }

            if ( address == TEST_PASS && data != 0 )
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
                    case TEST_BEGIN:
                        BeginTest();

                        break;

                    case TEST_FAIL:
                        FailTest();

                        break;
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
                throw new Exception( $"Finish test {m_CurrentTest} before starting test {m_TextBuilder}" );
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
