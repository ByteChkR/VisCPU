using System;

namespace VisCPU.Peripherals.Console
{

    public interface IConsoleBackend
    {
        uint CursorLeft { get; set; }
        uint CursorTop { get; set; }
        uint WindowWidth { get; set; }
        uint WindowHeight { get; set; }
        uint BufferWidth { get; set; }
        uint BufferHeight { get; set; }
        ConsoleColor ForeColor { get; set; }
        ConsoleColor BackColor { get; set; }
        bool IsInputAvailable();
        int Read();
        void Clear();
        void ResetColors();
        void Write( char c );
        void WriteDirect( Cpu cpu, uint start, uint length );
        void Write( uint number );
    }

}