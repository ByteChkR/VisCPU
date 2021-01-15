using System;
using System.Diagnostics;
using System.Text;

using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Peripherals.Benchmarking
{

    public class BenchmarkDevice : Peripheral
    {

        public class BenchmarkResultEvent : Event
        {

            private readonly string m_Name;
            private TimeSpan m_Time;

            public override string EventKey => "benchmark-run-result";

            #region Public

            public BenchmarkResultEvent( string name, TimeSpan time )
            {
                m_Name = name;
                m_Time = time;
            }

            public override string ToString()
            {
                return $"{m_Name}: {m_Time.Milliseconds} ms";
            }

            #endregion

        }

        private const uint DEVICE_PRESENT = 0xFFFF4000;
        private const uint RUN_BEGIN = 0xFFFF4001;
        private const uint RUN_END = 0xFFFF4002;
        private const uint RUN_CLEAR_NAME = 0xFFFF4003;
        private const uint RUN_SET_NAME = 0xFFFF4004;

        private readonly StringBuilder m_BenchmarkName = new StringBuilder();
        private readonly Stopwatch m_StopWatch = new Stopwatch();

        #region Public

        public override bool CanRead( uint address )
        {
            return address == DEVICE_PRESENT;
        }

        public override bool CanWrite( uint address )
        {
            return address == RUN_SET_NAME || address == RUN_CLEAR_NAME || address == RUN_END || address == RUN_BEGIN;
        }

        public override uint ReadData( uint address )
        {
            if ( address == DEVICE_PRESENT )
            {
                return 1;
            }

            throw new NotImplementedException();
        }

        public override void WriteData( uint address, uint data )
        {
            if ( address == RUN_SET_NAME )
            {
                m_BenchmarkName.Append( ( char ) data );
            }
            else if ( address == RUN_END )
            {
                StopTimer();
            }
            else if ( address == RUN_BEGIN )
            {
                BeginTimer();
            }
            else if ( address == RUN_CLEAR_NAME )
            {
                m_BenchmarkName.Clear();
            }
            else
            {
                throw new Exception( "Invalid use of Benchmark Device" );
            }
        }

        #endregion

        #region Private

        private void BeginTimer()
        {
            if ( m_StopWatch.IsRunning )
            {
                throw new Exception( "Benchmark Run Already Running, Finish the Benchmark to start the next one" );
            }

            m_StopWatch.Restart();
        }

        private void PrintResult()
        {
            EventManager.SendEvent( new BenchmarkResultEvent( m_BenchmarkName.ToString(), m_StopWatch.Elapsed ) );
        }

        private void StopTimer()
        {
            if ( !m_StopWatch.IsRunning )
            {
                throw new Exception( "No Benchmark Run running." );
            }

            m_StopWatch.Stop();
            PrintResult();
        }

        #endregion

    }

}
