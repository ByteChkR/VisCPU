namespace VisCPU.Utility.SharedBase
{

    public abstract class AToken
    {

        public int Length { get; }

        public string OriginalText { get; }

        #region Unity Event Functions

        public int Start { get; }

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
