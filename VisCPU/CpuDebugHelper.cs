using System.Collections.Generic;
using VisCPU.Utility.SharedBase;

namespace VisCPU
{

    public static class CpuDebugHelper
    {
        private static readonly List < LinkerInfo > infos = new List < LinkerInfo >();

        public static IEnumerable < LinkerInfo > LoadedSymbols => infos;

        #region Public

        public static void LoadSymbols( LinkerInfo info )
        {
            infos.Add( info );
        }

        public static void LoadSymbols( string binary )
        {
            LoadSymbols( LinkerInfo.Load( binary ) );
        }

        #endregion
    }

}
