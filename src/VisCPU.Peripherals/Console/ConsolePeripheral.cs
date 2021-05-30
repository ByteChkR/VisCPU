using System;
using System.Collections;
using System.Collections.Generic;

using VisCPU.Peripherals.Events;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;

namespace VisCPU.Peripherals.Console
{

    public class ConsolePeripheral : Peripheral
    {

        private readonly IConsoleBackend m_Backend = new DefaultConsoleBackend();
        private readonly ConsolePeripheralSettings m_Settings;

        public override string PeripheralName => "Console Peripheral";

        public override PeripheralType PeripheralType => PeripheralType.Console;

        public override uint PresentPin => m_Settings.InterfacePresentPin;

        public override uint AddressRangeStart => m_Settings.InterfacePresentPin;

        public override uint AddressRangeEnd => m_Settings.BufHeightAddr;

        private readonly Stack < uint > m_DirectWriteArgStack = new Stack<uint>();
        public ConsolePeripheral() : this(SettingsManager.GetSettings<ConsolePeripheralSettings>())
        {
        }

        public ConsolePeripheral(ConsolePeripheralSettings settings)
        {
            m_Settings = settings;
        }

        public override void Reset()
        {
        }

        public override uint ReadData(uint address)
        {
            if (address == m_Settings.InterfacePresentPin)
            {
                return 1u;
            }

            if (address == m_Settings.InputAvailableAddress)
            {
                uint v = m_Backend.IsInputAvailable() ? 1u : 0u;

                return v;
            }

            if (address == m_Settings.ReadInputAddress)
            {
                return (uint)m_Backend.Read();
            }
            

            if (address == m_Settings.CursorLeftAddr)
            {
                return m_Backend.CursorLeft;
            }

            if (address == m_Settings.CursorTopAddr)
            {
                return m_Backend.CursorTop;
            }

            if (address == m_Settings.WidthAddr)
            {
                return m_Backend.WindowWidth;
            }

            if (address == m_Settings.HeightAddr)
            {
                return m_Backend.WindowHeight;
            }

            if (address == m_Settings.BufWidthAddr)
            {
                return m_Backend.BufferWidth;
            }

            if (address == m_Settings.BufHeightAddr)
            {
                return m_Backend.BufferHeight;
            }

            if (address == m_Settings.BackColorAddr)
            {
                return (uint)m_Backend.BackColor;
            }

            if (address == m_Settings.ForeColorAddr)
            {
                return (uint)m_Backend.ForeColor;
            }



            EventManager<WarningEvent>.SendEvent(new InvalidPeripheralReadEvent(address, this));

            return 0;
        }

        public override void WriteData(uint address, uint data)
        {
            if (m_Settings.WriteOutputAddress == address)
            {
                m_Backend.Write((char)data);
            }
            else if (m_Settings.WriteNumOutputAddress == address)
            {
                m_Backend.Write(data);
            }
            else if ( m_Settings.WriteDirectAddress == address )
            {
                if ( m_DirectWriteArgStack.Count != 1 )
                {
                    m_DirectWriteArgStack.Push( data );
                }
                else
                {
                    m_Backend.WriteDirect(AttachedCpu, m_DirectWriteArgStack.Pop(), data);
                }
            }
            else if (m_Settings.InterfaceClearPin == address)
            {
                m_Backend.Clear();
            }
            else if (address == m_Settings.CursorLeftAddr)
            {
                m_Backend.CursorLeft = data;
            }
            else if (address == m_Settings.CursorTopAddr)
            {
                m_Backend.CursorTop = data;
            }
            else if (address == m_Settings.WidthAddr)
            {
                m_Backend.WindowWidth = data;
            }
            else if (address == m_Settings.HeightAddr)
            {
                m_Backend.WindowHeight = data;
            }
            else if (address == m_Settings.BufWidthAddr)
            {
                m_Backend.BufferWidth = data;
            }
            else if (address == m_Settings.BufHeightAddr)
            {
                m_Backend.BufferHeight = data;
            }
            else if (address == m_Settings.BackColorAddr)
            {
                m_Backend.BackColor = (ConsoleColor)data;
            }
            else if (address == m_Settings.ForeColorAddr)
            {
                m_Backend.ForeColor = (ConsoleColor)data;
            }
            else if (address == m_Settings.ResetColorAddr)
            {
                m_Backend.ResetColors();
            }
            else
            {
                EventManager<WarningEvent>.SendEvent(new InvalidPeripheralWriteEvent(address, data, this));
            }
        }


    }

}
