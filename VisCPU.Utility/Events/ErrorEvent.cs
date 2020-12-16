namespace VisCPU.Utility.Events
{

    public class ErrorEvent : Event
    {
        public string Message { get; }
        public override string EventKey { get; }
        public bool CanContinue { get; }
        public ErrorEvent(string errMessage, string eventKey, bool canContinue)
        {
            Message = errMessage;
            EventKey = eventKey;
            CanContinue = canContinue;
        }
    }

}