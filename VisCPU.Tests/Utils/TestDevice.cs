using System;
using System.Text;

namespace VisCPU.Tests
{

    public class TestDevice : Peripheral
    {

        public event Action<string> OnPass;
        public event Action<string, string> OnFail;

        private const uint TEST_BEGIN = 0xFFFF2000;
        private const uint TEST_FAIL = 0xFFFF2001;
        private const uint TEST_PASS = 0xFFFF2002;
        private const uint TEST_DEVICE_PRESENT = 0xFFFF2003;
        private uint CurrentCommand;

        private string CurrentTest;
        private readonly StringBuilder textBuilder = new StringBuilder();

        public override bool CanWrite(uint address)
        {
            return address == TEST_BEGIN || address == TEST_FAIL || address == TEST_PASS;
        }

        public override bool CanRead(uint address)
        {
            return address== TEST_DEVICE_PRESENT;
        }

        private void BeginTest()
        {
            if (CurrentTest != null) 
                throw new Exception($"Finish test {CurrentTest} before starting test {textBuilder}");
            CurrentTest = textBuilder.ToString();
        }

        private void PassTest()
        {
            OnPass?.Invoke(CurrentTest);
            CurrentTest = null;
        }

        private void FailTest()
        {
            string testName = CurrentTest;
            string desc = textBuilder.ToString();
            CurrentTest = null;
            CurrentCommand = 0;
            textBuilder.Clear();
            OnFail?.Invoke(testName, desc);
        }

        public override void WriteData(uint address, uint data)
        {
            if (CurrentCommand  != 0&& CurrentCommand != address)
            {
                throw new Exception($"Finish Command {CurrentCommand} before starting {address} command");
            }

            if (address == TEST_PASS && data != 0)
            {
                PassTest();
                return;
            }

            CurrentCommand = address;

            bool runCommand = false;
            if (CurrentCommand != 0)
            {
                if (data != 0)
                {
                    textBuilder.Append((char)data);
                }
                else
                {
                    runCommand = true;
                }
            }

            if (runCommand)
            {
                switch (address)
                {
                    case TEST_BEGIN:
                        BeginTest();
                        break;
                    case TEST_FAIL:
                        FailTest();
                        break;
                }

                CurrentCommand = 0;
                textBuilder.Clear();
            }
        }

        public override uint ReadData(uint address)
        {
            if ( address == TEST_DEVICE_PRESENT )
                return 1;
            throw new NotImplementedException();
        }

    }

}
