
using System;

namespace Utility.ExtPP.Plugins
{
    public class ErrorException : Exception
    {

        public ErrorException(string message) : base(message)
        {
        }

    }
}