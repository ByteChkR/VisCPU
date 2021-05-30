using System;
using System.Linq;
using System.Text;

namespace VisCPU.Peripherals.Console
{
    using System;

    public class DefaultConsoleBackend:IConsoleBackend
    {

        private readonly  StringBuilder m_Sb = new StringBuilder();
        public uint CursorLeft
        {
            get => ( uint ) System.Console.CursorLeft;
            set => System.Console.CursorLeft = (int)value;
        }

        public uint CursorTop
        {
            get => (uint)System.Console.CursorTop;
            set => System.Console.CursorTop = (int)value;
        }

        public uint WindowWidth
        {
            get => (uint)System.Console.WindowWidth;
            set => System.Console.WindowWidth = (int)value;
        }


        public uint WindowHeight
        {
            get => (uint)System.Console.WindowHeight;
            set => System.Console.WindowHeight = (int)value;
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
            get => System.Console.ForegroundColor;
            set => System.Console.ForegroundColor = value;
        }

        public ConsoleColor BackColor
        {
            get => System.Console.BackgroundColor;
            set => System.Console.BackgroundColor = value;
        }

        public bool IsInputAvailable()
        {
            return System.Console.KeyAvailable;
        }

        public int Read()
        {
            return System.Console.Read();
        }

        public void Clear()
        {
            System.Console.Clear();
        }

        public void ResetColors()
        {
            System.Console.ResetColor();
        }

        public void Write( char c )
        {
            System.Console.Write( c );
        }

        public void WriteDirect( Cpu cpu, uint start, uint length )
        {
            Memory.Memory m = cpu.MemoryBus.GetPeripherals < Memory.Memory >().
                                  First( x => x.ContainsInRange( start ) && x.ContainsInRange( start + length ) );

            uint[] data = m.GetInternalBuffer();
            int off = (int)start - (int)m.AddressRangeStart;

            m_Sb.Clear();
            for ( int i = off; i < off + length; i++ )
            {
                m_Sb.Append( ( char ) data[i] );
            }

            System.Console.Write(m_Sb.ToString() );
        }

        public void Write( uint number )
        {
            System.Console.Write( number.ToString() );
        }

    }

}