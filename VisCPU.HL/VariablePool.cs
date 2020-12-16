using VisCPU.HL.Compiler;

namespace VisCPU.HL
{

    public abstract class VariablePool
    {

        public abstract ExpressionTarget Get();

        public abstract void Release( ExpressionTarget target );

    }

}