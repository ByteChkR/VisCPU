using System;

namespace VisCPU.Utility.ArgumentParser
{

    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true )]
    public class ArgumentAttribute : Attribute
    {

        public string Name;

    }

}
