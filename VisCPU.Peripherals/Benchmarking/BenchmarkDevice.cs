﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;

namespace VisCPU.Peripherals.Benchmarking
{
    public class BenchmarkDevice : Peripheral
    {

        public class BenchmarkResultEvent : Event
        {

            public override string EventKey => "benchmark-run-result";

            private string Name;
            private TimeSpan Time;
            public BenchmarkResultEvent(string name, TimeSpan time)
            {
                Name = name;
                Time = time;
            }

            public override string ToString()
            {
                return $"{Name}: {Time.Milliseconds} ms";
            }

        }

        private const uint DEVICE_PRESENT = 0xFFFF4000;
        private const uint RUN_BEGIN = 0xFFFF4001;
        private const uint RUN_END = 0xFFFF4002;
        private const uint RUN_CLEAR_NAME = 0xFFFF4003;
        private const uint RUN_SET_NAME = 0xFFFF4004;

        private readonly StringBuilder benchmarkName = new StringBuilder();
        private readonly Stopwatch stopWatch = new Stopwatch();

        #region Public

        public override bool CanRead(uint address)
        {
            return address == DEVICE_PRESENT;
        }

        public override bool CanWrite(uint address)
        {
            return address == RUN_SET_NAME || address == RUN_CLEAR_NAME || address == RUN_END || address == RUN_BEGIN;
        }

        public override uint ReadData(uint address)
        {
            if (address == DEVICE_PRESENT)
            {
                return 1;
            }

            throw new NotImplementedException();
        }

        public override void WriteData(uint address, uint data)
        {
            if (address == RUN_SET_NAME)
            {
                benchmarkName.Append((char)data);
            }
            else if (address == RUN_END)
            {
                StopTimer();
            }
            else if (address == RUN_BEGIN)
            {
                BeginTimer();
            }
            else if (address == RUN_CLEAR_NAME)
            {
                benchmarkName.Clear();
            }
            else
            {
                throw new Exception("Invalid use of Benchmark Device");
            }
        }

        #endregion

        #region Private

        private void BeginTimer()
        {
            if (stopWatch.IsRunning)
                throw new Exception("Benchmark Run Already Running, Finish the Benchmark to start the next one");

            stopWatch.Restart();
        }

        private void StopTimer()
        {
            if (!stopWatch.IsRunning)
                throw new Exception("No Benchmark Run running.");

            stopWatch.Stop();
            PrintResult();
        }

        private void PrintResult()
        {
            EventManager.SendEvent( new BenchmarkResultEvent( benchmarkName.ToString(), stopWatch.Elapsed ) );
            //Log($"Benchmark {benchmarkName}: {stopWatch.ElapsedMilliseconds} ms");
        }

        #endregion

    }
}
