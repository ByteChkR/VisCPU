using System;
using System.Collections.Generic;

using VisCPU.Utility.Logging;

namespace VisCPU.Utility.Events
{

    public abstract class VisBase
    {

        protected abstract LoggerSystems SubSystem { get; }

        public virtual void Log( string message )
        {
            Logger.LogMessage( SubSystem, message );
        }
        
    }
    public class FileNotFoundEvent : ErrorEvent
    {

        private const string EVENT_KEY = "file-not-found";
        public FileNotFoundEvent(string file, bool canContinue) : base($"The file '{file}' could not be found.", EVENT_KEY, canContinue)
        {
        }

    }

    public class FileInvalidEvent : ErrorEvent
    {

        private const string EVENT_KEY = "file-invalid";
        public FileInvalidEvent(string file, bool canContinue) : base($"The file '{file}' is invalid.", EVENT_KEY, canContinue)
        {
        }

    }

    public abstract class Event
    {
        public abstract string EventKey { get; }
    }

    public class WarningEvent : Event
    {

        public string Message { get; }
        public override string EventKey { get; }
        public WarningEvent(string message, string eventKey)
        {
            Message = message;
            EventKey = eventKey;
        }

    }

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

    public static class EventManager
    {

        public static void Initialize()
        {
            EventManager<ErrorEvent>.OnEventReceive += EventManagerOnErrorEventReceive;
            EventManager<WarningEvent>.OnEventReceive += EventManagerOnWarningEventReceive;
        }

        private static void EventManagerOnWarningEventReceive( WarningEvent obj )
        {
            Console.WriteLine( $"[{obj.EventKey}] {obj.Message}" );
        }

        private static void EventManagerOnErrorEventReceive( ErrorEvent obj )
        {
            throw new Exception( $"[{obj.EventKey}] {obj.Message}" );
        }

        public static event Action<Event> OnEventReceive;
        public static void SendEvent(Event eventItem)
        {
            OnEventReceive?.Invoke(eventItem);
        }
    }

    public static class EventManager < T > where T : Event
    {

        public static event Action < T > OnEventReceive;

        public static void SendEvent(T eventItem)
        {
            OnEventReceive?.Invoke( eventItem );
            EventManager.SendEvent( eventItem );
        }

    }

}
