using VisCPU.Utility.IO.DataStore;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL
{

    public class ExpressionParser : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.HlParser;

        #region Public

        public HlCompilation Parse( string expr, string dir, BuildDataStore store )
        {
            HlCompilation c = new HlCompilation( expr, dir, store );

            return c;
        }

        #endregion

    }

}
