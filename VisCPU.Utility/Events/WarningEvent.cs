namespace VisCPU.Utility.Events
{
    public class WarningEvent : Event
    {
        #region Public

        public WarningEvent(string message, string eventKey)
        {
            Message = message;
            EventKey = eventKey;
        }

        #endregion

        public string Message { get; }

        public override string EventKey { get; }
    }
}