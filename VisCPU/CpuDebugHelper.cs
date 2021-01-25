using System.Collections.Generic;
using VisCPU.Utility.SharedBase;

namespace VisCPU
{

    public static class CpuDebugHelper
    {
        private static readonly List < LinkerInfo > s_Infos = new List < LinkerInfo >();

        public static IEnumerable < LinkerInfo > LoadedSymbols => s_Infos;

        #region Public

        public static void LoadSymbols( LinkerInfo info )
        {
            s_Infos.Add( info );
        }

        public static void LoadSymbols( string binary )
        {
            LoadSymbols( LinkerInfo.Load( binary ) );
        }

        #endregion
    }

}
