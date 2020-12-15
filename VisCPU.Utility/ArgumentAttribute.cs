using System;

namespace viscc
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ArgumentAttribute : Attribute
    {

        public string Name;

    }
}