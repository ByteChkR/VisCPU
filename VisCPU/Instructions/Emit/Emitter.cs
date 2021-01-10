using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Instructions.Emit
{

    public abstract class Emitter < EmitType > : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.Emit;

        #region Public

        public abstract EmitType Emit( string instructionKey, params string[] arguments );

        #endregion

    }

}
