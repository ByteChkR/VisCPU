namespace VisCPU.Utility
{

    public abstract class AToken
    {

        public readonly int Length;
        public readonly string OriginalText;

        #region Unity Event Functions

        public readonly int Start;

        #endregion

        #region Public

        public abstract string GetValue();

        public override string ToString()
        {
            return $"[{base.ToString()}]" + GetValue();
        }

        #endregion

        #region Protected

        protected AToken( string originalText, int start, int length )
        {
            OriginalText = originalText;
            Start = start;
            Length = length;
        }

        #endregion

    }

}
