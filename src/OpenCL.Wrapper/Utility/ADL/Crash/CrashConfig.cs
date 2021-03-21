using System;

using Utility.ADL.Configs;

namespace Utility.ADL.Crash
{
    [Serializable]
    public class CrashConfig : AbstractADLConfig
    {

        public bool ShortenCrashInfo { get; set; }

        public override AbstractADLConfig GetStandard()
        {
            return new CrashConfig();
        }

    }
}