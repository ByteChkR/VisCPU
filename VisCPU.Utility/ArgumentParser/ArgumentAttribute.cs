using System;

namespace VisCPU
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ArgumentAttribute : Attribute
    {

        public string Name;

    }
}