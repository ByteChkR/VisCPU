using System;

namespace VisCPU.Tests.Utils
{

    public class TestDeviceException : Exception
    {
        public TestDeviceException(string msg) : base(msg) { }
    }

}