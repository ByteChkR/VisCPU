namespace VisCPU.Utility.EventSystem.Events
{

    public class ErrorEvent : Event
    {

        public string Message { get; }

        public override string EventKey { get; }

        public bool CanContinue { get; }

        #region Public

        public ErrorEvent( string errMessage, string eventKey, bool canContinue )
        {
            Message = errMessage;
            EventKey = eventKey;
            CanContinue = canContinue;
        }

        #endregion

    }

}
