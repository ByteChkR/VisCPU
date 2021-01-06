using System;

namespace VisCPU.Utility.ArgumentParser
{

    [AttributeUsage( AttributeTargets.Field, AllowMultiple = true )]
    public class ArgumentAttribute : Attribute
    {

        public string Name;

    }

}
