using System;
using VisCPU.Peripherals.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;

namespace VisCPU.Peripherals.Console
{

    public class ConsoleInterface : Peripheral
    {
        private readonly ConsoleInterfaceSettings m_Settings;

        public override string PeripheralName => "Console Management Device";

        public override PeripheralType PeripheralType => PeripheralType.ConsoleManagement;

        public override uint PresentPin => m_Settings.InterfacePresentPin;

        #region Unity Event Functions

        public override void Reset()
        {
        }

        #endregion

        #region Public

        public ConsoleInterface()
        {
            m_Settings = SettingsManager.GetSettings < ConsoleInterfaceSettings >();
        }

        public override bool CanRead( uint address )
        {
            return m_Settings.Any( address );
        }

        public override bool CanWrite( uint address )
        {
            return m_Settings.Any( address );
        }

        public override uint ReadData( uint address )
        {
            if ( address == m_Settings.InterfacePresentPin )
            {
                return 1;
            }

            if ( address == m_Settings.CursorLeftAddr )
            {
                return ( uint ) System.Console.CursorLeft;
            }

            if ( address == m_Settings.CursorTopAddr )
            {
                return ( uint ) System.Console.CursorTop;
            }

            if (address == m_Settings.WidthAddr)
            {
                return (uint)System.Console.WindowWidth;
            }

            if (address == m_Settings.HeightAddr)
            {
                return (uint)System.Console.WindowHeight;
            }

            if (address == m_Settings.BufWidthAddr)
            {
                return (uint)System.Console.BufferWidth;
            }

            if (address == m_Settings.BufHeightAddr)
            {
                return (uint)System.Console.BufferHeight;
            }

            if ( address == m_Settings.BackColorAddr )
            {
                return ( uint ) System.Console.BackgroundColor;
            }

            if ( address == m_Settings.ForeColorAddr )
            {
                return ( uint ) System.Console.ForegroundColor;
            }

            EventManager < WarningEvent >.SendEvent( new InvalidPeripheralReadEvent( address, this ) );

            return 0;
        }

        public override void WriteData( uint address, uint data )
        {
            if ( address == m_Settings.CursorLeftAddr )
            {
                System.Console.CursorLeft = ( int ) data;
            }
            else if ( address == m_Settings.CursorTopAddr )
            {
                System.Console.CursorTop = ( int ) data;
            }
            else if (address == m_Settings.WidthAddr)
            {
                System.Console.WindowWidth = (int)data;
            }
            else if (address == m_Settings.HeightAddr)
            {
                System.Console.WindowHeight = (int)data;
            }
            else if (address == m_Settings.BufWidthAddr)
            {
                System.Console.BufferWidth = (int)data;
            }
            else if (address == m_Settings.BufHeightAddr)
            {
                System.Console.BufferHeight = (int)data;
            }
            else if ( address == m_Settings.BackColorAddr )
            {
                System.Console.BackgroundColor = ( ConsoleColor ) data;
            }
            else if ( address == m_Settings.ForeColorAddr )
            {
                System.Console.ForegroundColor = ( ConsoleColor ) data;
            }
            else if ( address == m_Settings.ResetColorAddr )
            {
                System.Console.ResetColor();
            }
            else
            {
                EventManager < WarningEvent >.SendEvent( new InvalidPeripheralWriteEvent( address, data, this ) );
            }
        }

        #endregion
    }

}
