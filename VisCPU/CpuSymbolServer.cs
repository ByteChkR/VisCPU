using System.Collections.Generic;
using VisCPU.Utility.SharedBase;

namespace VisCPU
{

    public class CpuSymbolServer
    {
        private readonly List < LinkerInfo > s_Infos = new List < LinkerInfo >();

        public IEnumerable < LinkerInfo > LoadedSymbols => s_Infos;

        #region Public

        public void LoadSymbols( LinkerInfo info )
        {
            s_Infos.Add( info );
        }

        public void LoadSymbols( string binary )
        {
            LoadSymbols( LinkerInfo.Load( binary ) );
        }

        #endregion
    }

}
