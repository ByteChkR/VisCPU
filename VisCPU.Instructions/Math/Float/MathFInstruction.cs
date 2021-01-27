namespace VisCPU.Instructions.Math.Float
{

    public abstract class MathFInstruction : MathInstruction
    {
        #region Public

        public abstract float Calculate( float a, float b );

        public override uint Calculate( uint a, uint b )
        {
            unsafe
            {
                float fa = *( float* ) &a;
                float fb = *( float* ) &b;
                float fr = Calculate( fa, fb );

                return *( uint* ) &fr;
            }
        }

        #endregion
    }

}
