using VisCPU.HL.Compiler;

namespace VisCPU.HL
{
    public abstract class VariablePool
    {
        #region Public

        public abstract ExpressionTarget Get();

        public abstract void Release(ExpressionTarget target);

        #endregion
    }
}