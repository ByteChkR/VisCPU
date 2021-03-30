using System;
using System.Collections.Generic;

namespace VisCPU.Console.Core.Subsystems.VM
{

    [Serializable]
    public class VMConfigs
    {
        public List < VMConfig > Configurations = new List < VMConfig > { new VMConfig() };
    }

}