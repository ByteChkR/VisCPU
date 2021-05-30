using System;
using System.Linq;
using System.Text;

namespace VisCPU.Peripherals.Console
{

    public class WindowsConsoleBackend : IConsoleBackend
    {

        private readonly StringBuilder m_Sb = new StringBuilder();

        private InputConsoleBox m_ConsoleBox = new InputConsoleBox(0, 0, (short)System.Console.WindowWidth, (short)(System.Console.WindowHeight - 1));
        private string m_LastInput;
        private int m_LastInputIndex;
        private bool m_Initialized = false;

        private void Initialize()
        {
            m_Initialized = true;
            System.Console.Clear();
            System.Console.WindowTop = 0;
            System.Console.WindowLeft = 0;
        }
        public uint CursorLeft
        {
            get => (uint)m_ConsoleBox.GetCursorPosition().Item1;
            set => m_ConsoleBox.SetCursorPosition((int)value, (int)CursorTop);
        }

        public uint CursorTop
        {
            get => (uint)m_ConsoleBox.GetCursorPosition().Item2;
            set => m_ConsoleBox.SetCursorPosition((int)CursorLeft, (int)value);
        }


        public uint WindowWidth
        {
            get => (uint)m_ConsoleBox.Width;
            set => ResizeConsoleBox((int)value, (int)WindowHeight);
        }


        public uint WindowHeight
        {
            get => (uint)m_ConsoleBox.Height;
            set => ResizeConsoleBox((int)WindowWidth, (int)value);
        }

        public uint BufferWidth
        {
            get => (uint)System.Console.BufferWidth;
            set => System.Console.BufferWidth = (int)value;
        }

        public uint BufferHeight
        {
            get => (uint)System.Console.BufferHeight;
            set => System.Console.BufferHeight = (int)value;
        }

        public ConsoleColor ForeColor
        {
            get => Convert(m_ConsoleBox.Foreground);
            set => m_ConsoleBox.SetForeColor(Convert(value));
        }

        public ConsoleColor BackColor
        {
            get => Convert(m_ConsoleBox.Background);
            set => m_ConsoleBox.SetBackColor(Convert(value));
        }
        private ConsoleColor Convert(InputConsoleBox.Colors color)
        {
            return (ConsoleColor)Enum.Parse(typeof(ConsoleColor), Enum.GetName(typeof(InputConsoleBox.Colors), color));
        }

        private InputConsoleBox.Colors Convert(ConsoleColor color)
        {
            return (InputConsoleBox.Colors)Enum.Parse(typeof(InputConsoleBox.Colors), Enum.GetName(typeof(ConsoleColor), color));
        }

        public bool IsInputAvailable()
        {
            if (!m_Initialized)
                Initialize();
            return System.Console.KeyAvailable;
        }

        public int Read()
        {
            if ( !m_Initialized )
                Initialize();
            if ( m_LastInput != null && m_LastInputIndex < m_LastInput.Length )
            {
                return m_LastInput[m_LastInputIndex++];
            }

            ( int l, int t ) = m_ConsoleBox.GetCursorPosition();
            System.Console.CursorLeft = l;
            System.Console.CursorTop = t;
            m_LastInputIndex = 1;
            m_LastInput = System.Console.ReadLine()+'\n';

            return m_LastInput[0];
        }

        public void Clear()
        {
            if (!m_Initialized)
                Initialize();
            m_ConsoleBox.Clear();
        }

        public void ResetColors()
        {
            if (!m_Initialized)
                Initialize();
            m_ConsoleBox.SetDefaultColor(m_ConsoleBox.ResetForeground, m_ConsoleBox.ResetBackground);
        }

        public void Write(char c)
        {
            if (!m_Initialized)
                Initialize();
            m_ConsoleBox.Write(new[] { c });
        }

        public void WriteDirect(Cpu cpu, uint start, uint length)
        {
            if (!m_Initialized)
                Initialize();
            Memory.Memory m = cpu.MemoryBus.GetPeripherals<Memory.Memory>().
                                  First(x => x.ContainsInRange(start) && x.ContainsInRange(start + length));

            uint[] data = m.GetInternalBuffer();
            int off = (int)start - (int)m.AddressRangeStart;

            m_Sb.Clear();
            for (int i = off; i < off + length; i++)
            {
                m_Sb.Append((char)data[i]);
            }

            m_ConsoleBox.Write(m_Sb.ToString());
        }

        public void Write(uint number)
        {
            if (!m_Initialized)
                Initialize();
            m_ConsoleBox.Write(number.ToString());
        }

        private void ResizeConsoleBox(int width, int height)
        {
            if (!m_Initialized)
                Initialize();
            m_ConsoleBox.Move(m_ConsoleBox.Left, m_ConsoleBox.Top, (short)width, (short)height);
        }

    }

}