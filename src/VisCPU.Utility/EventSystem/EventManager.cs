using System;
using System.Linq;

using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Utility.EventSystem
{

    public static class EventManager
    {

        public static event Action < Event > OnEventReceive;

        internal static EventManagerSettings Settings { get; private set; } = new EventManagerSettings();

        #region Public

        public static void RegisterDefaultHandlers()
        {
            if ( Settings == null )
            {
                Settings = new EventManagerSettings();
            }

            OnEventReceive += EventManagerOnEventReceive;
            EventManager < ErrorEvent >.OnEventReceive += EventManagerOnErrorEventReceive;
        }

        public static void ResetEventHandler()
        {
            OnEventReceive = null;
        }

        public static void SendEvent( Event eventItem )
        {
            OnEventReceive?.Invoke( eventItem );
        }

        public static void SetSettings( EventManagerSettings settings )
        {
            Settings = settings;
        }

        #endregion

        #region Private

        private static void EventManagerOnErrorEventReceive( ErrorEvent obj )
        {
            if ( Settings.Interactive )
            {
                Console.Write( $"Encountered Error Event: {obj.EventKey} Do you want to continue? [Y/n]" );

                if ( Console.ReadKey().Key == ConsoleKey.Y )
                {
                    throw new EventManagerException( $"[{obj.EventKey}] {obj.Message}" );
                }

                return;
            }

            if ( obj.CanContinue ||
                 Settings.IgnoredEvents.Contains( obj.EventKey ) )
            {
                return;
            }

            throw new EventManagerException( $"[{obj.EventKey}] {obj.Message}" );
        }

        private static void EventManagerOnEventReceive( Event obj )
        {
            if ( Settings.Interactive )
            {
                Console.Write( $"Encountered Event: {obj.EventKey} Do you want to continue? [Y/n]" );

                if ( Console.ReadKey().Key == ConsoleKey.Y )
                {
                    throw new EventManagerException( $"[{obj.EventKey}]" );
                }

                return;
            }

            if ( obj is WarningEvent wrn )
            {
                Console.WriteLine( $"[{wrn.EventKey}] {wrn.Message}" );
            }
            else if ( obj is ErrorEvent err )
            {
                Console.WriteLine( $"[{err.EventKey}] {err.Message}" );
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

        public static void ResetEventHandler()
        {
            OnEventReceive = null;
        }

        public static void SendEvent( T eventItem )
        {
            OnEventReceive?.Invoke( eventItem );
            EventManager.SendEvent( eventItem );
        }

        #endregion

    }

}
