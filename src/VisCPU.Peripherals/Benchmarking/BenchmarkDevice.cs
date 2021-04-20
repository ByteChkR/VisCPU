using System;
using System.Diagnostics;
using System.Text;

using VisCPU.Peripherals.Events;
using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

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
                return $"{m_Name}: {m_Time.TotalMilliseconds} ms";
            }

            #endregion

        }

        private const uint DevicePresent = 0xFFFF4000;
        private const uint RunBegin = 0xFFFF4001;
        private const uint RunEnd = 0xFFFF4002;
        private const uint RunClearName = 0xFFFF4003;
        private const uint RunSetName = 0xFFFF4004;

        private readonly StringBuilder m_BenchmarkName = new StringBuilder();
        private readonly Stopwatch m_StopWatch = new Stopwatch();

        public override string PeripheralName => "Benchmark Device";

        public override PeripheralType PeripheralType => PeripheralType.Custom;

        public override uint PresentPin => DevicePresent;

        #region Unity Event Functions

        public override void Reset()
        {
            m_StopWatch.Stop();
            m_BenchmarkName.Clear();
        }

        #endregion

        #region Public

        public override bool CanRead( uint address )
        {
            return address == DevicePresent;
        }

        public override bool CanWrite( uint address )
        {
            return address == RunSetName || address == RunClearName || address == RunEnd || address == RunBegin;
        }

        public override uint ReadData( uint address )
        {
            if ( address == DevicePresent )
            {
                return 1;
            }

            throw new NotImplementedException();
        }

        public override void WriteData( uint address, uint data )
        {
            if ( address == RunSetName )
            {
                m_BenchmarkName.Append( ( char ) data );
            }
            else if ( address == RunEnd )
            {
                StopTimer();
            }
            else if ( address == RunBegin )
            {
                BeginTimer();
            }
            else if ( address == RunClearName )
            {
                m_BenchmarkName.Clear();
            }
            else
            {
                EventManager < ErrorEvent >.SendEvent(
                                                      new InvalidBenchmarkDeviceUsageEvent(
                                                           $"Unrecognized Address: {address.ToHexString()}"
                                                          )
                                                     );
            }
        }

        #endregion

        #region Private

        private void BeginTimer()
        {
            if ( m_StopWatch.IsRunning )
            {
                EventManager < ErrorEvent >.SendEvent(
                                                      new InvalidBenchmarkDeviceUsageEvent(
                                                           "Benchmark Run Already Running, Finish the Benchmark to start the next one"
                                                          )
                                                     );

                return;
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
                EventManager < ErrorEvent >.SendEvent(
                                                      new InvalidBenchmarkDeviceUsageEvent(
                                                           "No Benchmark Run running."
                                                          )
                                                     );

                return;
            }

            m_StopWatch.Stop();
            PrintResult();
        }

        #endregion

    }

}
