namespace VisCPU.Utility.Events
{
    public class ErrorEvent : Event
    {
        #region Public

        public ErrorEvent(string errMessage, string eventKey, bool canContinue)
        {
            Message = errMessage;
            EventKey = eventKey;
            CanContinue = canContinue;
        }

        #endregion

        public string Message { get; }

        public override string EventKey { get; }

        public bool CanContinue { get; }
    }
}