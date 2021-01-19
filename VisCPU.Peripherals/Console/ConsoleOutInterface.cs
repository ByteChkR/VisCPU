using System;
using VisCPU.Peripherals.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.Console
{

    public class ConsoleOutInterface : Peripheral
    {

        private readonly ConsoleOutInterfaceSettings m_Settings;

        public Action<char> WriteConsoleChar { get; set; } = System.Console.Write;
        public Action ConsoleClear { get; set; } = System.Console.Clear;
        public Action<uint> WriteConsoleNum { get; set; } = x => System.Console.Write(x.ToString());

        #region Public

        public ConsoleOutInterface()
        {
            m_Settings = SettingsManager.GetSettings < ConsoleOutInterfaceSettings >();
        }

        public override bool CanRead( uint address )
        {
            return m_Settings.InterfacePresentPin == address;
        }

        public override bool CanWrite( uint address )
        {
            return address == m_Settings.WriteOutputAddress ||
                   m_Settings.WriteNumOutputAddress == address ||
                   m_Settings.InterfaceClearPin == address;
        }

        public override uint ReadData( uint address )
        {
            if ( address == m_Settings.InterfacePresentPin )
            {
                return 1;
            }

            EventManager < WarningEvent >.SendEvent( new InvalidPeripheralReadEvent( address, this ) );

            return 0;
        }

        public override void WriteData( uint address, uint data )
        {
            if ( CanWrite( address ) )
            {
                if ( m_Settings.InterfaceClearPin == address )
                {
                    ConsoleClear();
                }
                else if ( m_Settings.WriteNumOutputAddress == address )
                {
                    WriteConsoleNum(data);
                }
                else
                {
                    WriteConsoleChar((char) data);
                }
            }
        }

        #endregion

    }

}
