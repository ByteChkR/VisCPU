using VisCPU.Utility.IO;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL
{

    public class ExpressionParser : VisBase
    {
        protected override LoggerSystems SubSystem => LoggerSystems.HlParser;

        #region Public

        public HLCompilation Parse( string expr, string dir, BuildDataStore store )
        {
            HLCompilation c = new HLCompilation( expr, dir, store );

            return c;
        }

        #endregion
    }

}
