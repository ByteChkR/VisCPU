using System;
using System.Collections.Generic;

namespace VisCPU.Utility.Events
{

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
