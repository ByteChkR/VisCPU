using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Instructions.Emit
{

    public abstract class Emitter < EmitType > : VisBase, IEmitter
    {
        protected override LoggerSystems SubSystem => LoggerSystems.Emit;

        #region Public

        public abstract EmitType Emit( string instructionKey, params string[] arguments );

        #endregion

        #region Private

        object IEmitter.Emit( string instructionKey, params string[] arguments )
        {
            return Emit( instructionKey, arguments );
        }

        #endregion
    }

}
