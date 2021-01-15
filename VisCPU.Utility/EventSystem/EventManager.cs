using System;

using VisCPU.Utility.Events;

namespace VisCPU.Utility.EventSystem
{

    public static class EventManager
    {

        public static event Action < Event > OnEventReceive;

        #region Public

        public static void RegisterDefaultHandlers()
        {
            OnEventReceive += EventManagerOnEventReceive;
            EventManager < ErrorEvent >.OnEventReceive += EventManagerOnErrorEventReceive;
        }

        public static void SendEvent( Event eventItem )
        {
            OnEventReceive?.Invoke( eventItem );
        }

        #endregion

        #region Private

        private static void EventManagerOnErrorEventReceive( ErrorEvent obj )
        {
            if ( obj.CanContinue )
            {
                return;
            }

            throw new Exception( $"[{obj.EventKey}] {obj.Message}" );
        }

        private static void EventManagerOnEventReceive( Event obj )
        {
            if ( obj is WarningEvent wrn )
            {
                Console.WriteLine( $"[{wrn.EventKey}] {wrn.Message}" );
            }
            else
            {
                Console.WriteLine( $"[{obj.EventKey}] {obj}" );
            }
        }

        #endregion

    }

    public static class EventManager < T > where T : Event
    {

        public static event Action < T > OnEventReceive;

        #region Public

        public static void SendEvent( T eventItem )
        {
            OnEventReceive?.Invoke( eventItem );
            EventManager.SendEvent( eventItem );
        }

        #endregion

    }

}
