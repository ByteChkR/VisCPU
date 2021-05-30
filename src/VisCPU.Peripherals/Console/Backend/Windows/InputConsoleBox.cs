using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Win32.SafeHandles;

namespace VisCPU.Peripherals.Console
{

    public class InputConsoleBox
    {

        public enum AutoDrawMode
        {

            OFF,
            NEWLINE,
            ALWAYS

        }

        [Flags]
        public enum Colors : int
        {

            Black = 0x0000,
            DarkBlue = 0x0001,
            DarkGreen = 0x0002,
            DarkCyan = DarkGreen | DarkBlue,
            DarkRed = 0x0004,
            DarkMagenta = DarkRed | DarkBlue,
            DarkYellow = DarkRed | DarkGreen,
            Gray = HIGH_INTENSITY,
            DarkGray = DarkBlue | DarkGreen | DarkRed,
            Blue = DarkBlue | HIGH_INTENSITY,
            Green = DarkGreen | HIGH_INTENSITY,
            Cyan = DarkCyan | HIGH_INTENSITY,
            Red = DarkRed | HIGH_INTENSITY,
            Magenta = DarkMagenta | HIGH_INTENSITY,
            Yellow = DarkYellow | HIGH_INTENSITY,
            White = DarkGray | HIGH_INTENSITY,

        }

        /// <summary>
        ///     Automatically draw to console.
        ///     Unset this if you want to manually control when (and what order) boxes are writen to consoles - or you want to
        ///     batch some stuff.
        ///     You must manually call <c>Draw()</c> to write to console.
        /// </summary>
        public AutoDrawMode AutoDraw = AutoDrawMode.NEWLINE;

        [StructLayout( LayoutKind.Explicit )]
        private struct CharInfo
        {

            [FieldOffset( 0 )]
            public CharUnion Char;

            [FieldOffset( 2 )]
            public ushort Attributes;

            public CharInfo( char @char, ushort attributes )
            {
                Char = new CharUnion();
                Char.UnicodeChar = @char;
                Attributes = attributes;
            }

        }

        [StructLayout( LayoutKind.Explicit )]
        private struct CharUnion
        {

            [FieldOffset( 0 )]
            public char UnicodeChar;

            [FieldOffset( 0 )]
            public byte AsciiChar;

        }

        [StructLayout( LayoutKind.Sequential )]
        private struct Coord
        {

            public short X;
            public short Y;

            public Coord( short x, short y )
            {
                X = x;
                Y = y;
            }

        };

        [StructLayout( LayoutKind.Sequential )]
        private struct SmallRect
        {

            public short Left;
            public short Top;
            public short Right;
            public short Bottom;

        }

        private static SafeFileHandle s_SafeFileHandle;
        private const uint STD_OUTPUT_HANDLE = unchecked( ( uint ) -11 );
        private const uint STD_ERROR_HANDLE = unchecked( ( uint ) -12 );

        private const int HIGH_INTENSITY = 0x0008;
        private const ushort COMMON_LVB_LEADING_BYTE = 0x0100;
        private const ushort COMMON_LVB_TRAILING_BYTE = 0x0200;
        private const ushort COMMON_LVB_GRID_HORIZONTAL = 0x0400;
        private const ushort COMMON_LVB_GRID_LVERTICAL = 0x0800;
        private const ushort COMMON_LVB_GRID_RVERTICAL = 0x1000;
        private const ushort COMMON_LVB_REVERSE_VIDEO = 0x4000;
        private const ushort COMMON_LVB_UNDERSCORE = 0x8000;
        private const ushort COMMON_LVB_SBCSDBCS = 0x0300;

        private CharInfo[] m_Buffer;
        private List < CharInfo > m_TmpBuffer;
        private short m_Left;
        private short m_Top;
        private short m_Width;
        private short m_Height;
        private ushort m_DefaultColor;
        private int m_CursorLeft;
        private int m_CursorTop;
        private string m_CurrentInputBuffer = "";
        private string m_InputPrompt;
        private int m_InputCursorPos = 0;

        private int m_InputFrameStart = 0;

        // Not used because COMMON_LVB_UNDERSCORE doesn't work
        //private bool _inputCursorState = false;
        //private int _inputCursorStateChange = 0;
        private int m_CursorBlinkLeft = 0;
        private int m_CursorBlinkTop = 0;

        public Colors ResetForeground { get; }

        public Colors ResetBackground { get; }

        public Colors Foreground { get; private set; }

        public Colors Background { get; private set; }

        public short Left => m_Left;

        public short Top => m_Top;

        public short Width => m_Width;

        public short Height => m_Height;

        public bool IsDirty { get; private set; }

        public string InputPrompt
        {
            get => m_InputPrompt;
            set
            {
                m_InputPrompt = value;
                ResetInput();
            }
        }

        #region Public

        public InputConsoleBox(
            short left,
            short top,
            short width,
            short height,
            Colors defaultForegroundColor = Colors.Gray,
            Colors defaultBackgroundColor = Colors.Black )
        {
            Move( left, top, width, height );

            ResetForeground = defaultForegroundColor;
            ResetBackground = defaultBackgroundColor;
            SetDefaultColor( defaultForegroundColor, defaultBackgroundColor );

            //SafeFileHandle h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if ( s_SafeFileHandle == null )
            {
                IntPtr stdOutputHandle = GetStdHandle( STD_OUTPUT_HANDLE );
                s_SafeFileHandle = new SafeFileHandle( stdOutputHandle, false );
            }

            Clear();
            Draw();
        }

        public void Clear()
        {
            for ( int y = 0; y < m_Height; y++ )
            {
                for ( int x = 0; x < m_Width; x++ )
                {
                    int i = y * m_Width + x;
                    m_Buffer[i].Char.UnicodeChar = ' ';
                    m_Buffer[i].Attributes = m_DefaultColor;
                }
            }

            IsDirty = true;

            // Update screen
            if ( AutoDraw != AutoDrawMode.OFF )
            {
                Draw();
            }
        }

        public void Draw()
        {
            IsDirty = false;

            SmallRect rect = new SmallRect()
                             {
                                 Left = m_Left,
                                 Top = m_Top,
                                 Right = ( short ) ( m_Left + m_Width ),
                                 Bottom = ( short ) ( m_Top + m_Height )
                             };

            bool b = WriteConsoleOutput(
                                        s_SafeFileHandle,
                                        m_Buffer,
                                        new Coord( m_Width, m_Height ),
                                        new Coord( 0, 0 ),
                                        ref rect
                                       );
        }

        public (int, int) GetCursorPosition()
        {
            return ( m_CursorLeft, m_CursorTop );
        }

        public void Move( short left, short top, short width, short height )
        {
            if ( left < 0 ||
                 top < 0 ||
                 left + width > System.Console.BufferWidth ||
                 top + height > System.Console.BufferHeight )
            {
                throw new Exception(
                                    string.Format(
                                                  "Attempting to create a box {0},{1}->{2},{3} that is out of buffer bounds 0,0->{4},{5}",
                                                  left,
                                                  top,
                                                  left + width,
                                                  top + height,
                                                  System.Console.BufferWidth,
                                                  System.Console.BufferHeight
                                                 )
                                   );
            }

            m_Left = left;
            m_Top = top;
            m_Width = width;
            m_Height = height;
            m_Buffer = new CharInfo[m_Width * m_Height];

            m_TmpBuffer =
                new List < CharInfo >(
                                      m_Width * m_Height
                                     ); // Assumption that we won't be writing much more than a screenful (backbufferfull) in every write operation
        }

        public string ReadLine()
        {
            System.Console.CursorVisible = false;
            Clear();
            ResetInput();

            while ( true )
            {
                Thread.Sleep( 50 );

                while ( System.Console.KeyAvailable )
                {
                    ConsoleKeyInfo key = System.Console.ReadKey( true );

                    switch ( key.Key )
                    {
                        case ConsoleKey.Enter:
                        {
                            string ret = m_CurrentInputBuffer;
                            m_InputCursorPos = 0;
                            m_CurrentInputBuffer = "";

                            return ret;

                            break;
                        }

                        case ConsoleKey.LeftArrow:
                        {
                            m_InputCursorPos = Math.Max( 0, m_InputCursorPos - 1 );

                            break;
                        }

                        case ConsoleKey.RightArrow:
                        {
                            m_InputCursorPos = Math.Min( m_CurrentInputBuffer.Length, m_InputCursorPos + 1 );

                            break;
                        }

                        case ConsoleKey.Backspace:
                        {
                            if ( m_InputCursorPos > 0 )
                            {
                                m_InputCursorPos--;
                                m_CurrentInputBuffer = m_CurrentInputBuffer.Remove( m_InputCursorPos, 1 );
                            }

                            break;
                        }

                        case ConsoleKey.Delete:
                        {
                            if ( m_InputCursorPos < m_CurrentInputBuffer.Length - 1 )
                            {
                                m_CurrentInputBuffer = m_CurrentInputBuffer.Remove( m_InputCursorPos, 1 );
                            }

                            break;
                        }

                        default:
                        {
                            int pos = m_InputCursorPos;

                            //if (_inputCursorPos == _currentInputBuffer.Length)
                            m_InputCursorPos++;
                            m_CurrentInputBuffer = m_CurrentInputBuffer.Insert( pos, key.KeyChar.ToString() );

                            break;
                        }
                    }

                    ResetInput();
                }

                // COMMON_LVB_UNDERSCORE doesn't work so we use Consoles default cursor
                //UpdateCursorBlink(false);
            }
        }

        public void SetBackColor( Colors background )
        {
            SetDefaultColor( Foreground, background );
        }

        public void SetCursorBlink( int left, int top, bool state )
        {
            System.Console.SetCursorPosition( left, top );
            System.Console.CursorVisible = state;
            //// Does not work
            //var i = (top * _width) + left;
            //if (state)
            //    _buffer[i].Attributes = (ushort)((int)_buffer[i].Attributes & ~(int)COMMON_LVB_UNDERSCORE);
            //else
            //    _buffer[i].Attributes = (ushort)((int)_buffer[i].Attributes | (int)COMMON_LVB_UNDERSCORE);

            //if (AutoDraw)
            //    Draw();
        }

        public void SetCursorPosition( int left, int top )
        {
            if ( left >= m_Width || top >= m_Height )
            {
                throw new Exception(
                                    string.Format(
                                                  "Position out of bounds attempting to set cursor at box pos {0},{1} when box size is only {2},{3}.",
                                                  left,
                                                  top,
                                                  m_Width,
                                                  m_Height
                                                 )
                                   );
            }

            m_CursorLeft = left;
            m_CursorTop = top;
        }

        public void SetDefaultColor( Colors foreground, Colors background )
        {
            Foreground = foreground;
            Background = background;
            m_DefaultColor = CombineColors( foreground, background );
        }

        public void SetForeColor( Colors foreground )
        {
            SetDefaultColor( foreground, Background );
        }

        public void Write( string text )
        {
            Write( text.ToCharArray() );
        }

        public void Write( char[] text )
        {
            IsDirty = true;
            m_TmpBuffer.Clear();
            bool newLine = false;
            bool hadNewLine = false;

            // Old-school! Could definitively have been done more easily with regex. :)
            int col = 0;
            int row = 0;
            int cursorI = m_CursorTop * m_Width + m_CursorLeft;

            for ( int i = 0; i < text.Length; i++ )
            {
                // Detect newline
                if ( text[i] == '\n' )
                {
                    newLine = true;
                }

                if ( text[i] == '\r' )
                {
                    newLine = true;

                    // Skip following \n
                    if ( i + 1 < text.Length && text[i] == '\n' )
                    {
                        i++;
                    }
                }

                // Keep track of column and row
                col++;

                if ( col == m_Width )
                {
                    col = 0;
                    row++;

                    if ( newLine ) // Last character was newline? Skip filling the whole next line with empty
                    {
                        newLine = false;
                        hadNewLine = true;

                        continue;
                    }
                }

                // If we are newlining we need to fill the remaining with blanks
                if ( newLine )
                {
                    hadNewLine = true;
                       newLine = false;

                    for ( int i2 = cursorI%m_Width+col; i2 <= m_Width; i2++ )
                    {
                        m_TmpBuffer.Add( new CharInfo( ' ', m_DefaultColor ) );
                    }

                    col = 0;
                    row++;
                    m_CursorLeft = 0;

                    continue;
                }

                if ( i >= text.Length )
                {
                    break;
                }

                // Add character
                m_TmpBuffer.Add( new CharInfo( text[i], m_DefaultColor ) );
            }



            // Get our end position
            int end = cursorI + m_TmpBuffer.Count;

            // If we are overflowing (scrolling) then we need to complete our last line with spaces (align buffer with line ending)
            if (end > m_Buffer.Length && col != 0)
            {
                for (int i = cursorI % m_Width + col + 1; i <= m_Width; i++)
                {
                    m_TmpBuffer.Add(new CharInfo(' ', m_DefaultColor));
                }

                //col = 0;
                //row++;
            }

            // Chop start of buffer to fit into destination buffer
            if ( m_TmpBuffer.Count > m_Buffer.Length )
            {
                m_TmpBuffer.RemoveRange( 0, m_TmpBuffer.Count - m_Buffer.Length );
            }

            // Convert to array so we can batch copy
            CharInfo[] tmpArray = m_TmpBuffer.ToArray();

            // Are we going to write outside of buffer?
            end = cursorI + m_TmpBuffer.Count;
            int scrollUp = 0;

            if ( end > m_Buffer.Length )
            {
                scrollUp = end - m_Buffer.Length;
            }

            // Scroll up
            if ( scrollUp > 0 )
            {
                Array.Copy( m_Buffer, scrollUp, m_Buffer, 0, m_Buffer.Length - scrollUp );
                cursorI -= scrollUp;
            }

            int lastPos = Math.Min( m_Buffer.Length, cursorI + tmpArray.Length );
            int firstPos = lastPos - tmpArray.Length;

            // Copy new data in
            Array.Copy( tmpArray, 0, m_Buffer, firstPos, tmpArray.Length );

            // Set new cursor position
            m_CursorLeft += col;
            m_CursorTop = Math.Min( m_Height, m_CursorTop + row );

            // Write to main buffer
            if (AutoDraw == AutoDrawMode.ALWAYS || 
                hadNewLine && AutoDraw== AutoDrawMode.NEWLINE)
            {
                Draw();
            }
        }

        public void WriteLine( string line, Colors fgColor, Colors bgColor )
        {
            ushort c = m_DefaultColor;
            m_DefaultColor = CombineColors( fgColor, bgColor );
            WriteLine( line );
            m_DefaultColor = c;
        }

        public void WriteLine( string line )
        {
            Write( line + "\n" );
        }

        #endregion

        #region Private

        private static ushort CombineColors( Colors foreColor, Colors backColor )
        {
            return ( ushort ) ( ( int ) foreColor + ( ( int ) backColor << 4 ) );
        }

        [DllImport( "Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        private static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs( UnmanagedType.U4 )]
            uint fileAccess,
            [MarshalAs( UnmanagedType.U4 )]
            uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs( UnmanagedType.U4 )]
            FileMode creationDisposition,
            [MarshalAs( UnmanagedType.U4 )]
            int flags,
            IntPtr template );

        [DllImport( "Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        private static extern IntPtr GetStdHandle( uint type );

        [DllImport( "kernel32.dll", SetLastError = true )]
        private static extern bool WriteConsoleOutput(
            SafeFileHandle hConsoleOutput,
            CharInfo[] lpBuffer,
            Coord dwBufferSize,
            Coord dwBufferCoord,
            ref SmallRect lpWriteRegion );

        private void ResetInput()
        {
            SetCursorPosition( 0, 0 );
            m_InputCursorPos = Math.Min( m_CurrentInputBuffer.Length, m_InputCursorPos );

            string inputPrompt = InputPrompt + "[" + m_CurrentInputBuffer.Length + "] ";

            // What is the max length we can write?
            int maxLen = m_Width - inputPrompt.Length;

            if ( maxLen < 0 )
            {
                return;
            }

            if ( m_InputCursorPos > m_InputFrameStart + maxLen )
            {
                m_InputFrameStart = m_InputCursorPos - maxLen;
            }

            if ( m_InputCursorPos < m_InputFrameStart )
            {
                m_InputFrameStart = m_InputCursorPos;
            }

            m_CursorBlinkLeft = inputPrompt.Length + m_InputCursorPos - m_InputFrameStart;

            //if (_currentInputBuffer.Length - _inputFrameStart < maxLen)
            //    _inputFrameStart--;

            // Write and pad the end
            string str = inputPrompt +
                         m_CurrentInputBuffer.Substring(
                                                        m_InputFrameStart,
                                                        Math.Min(
                                                                 m_CurrentInputBuffer.Length - m_InputFrameStart,
                                                                 maxLen
                                                                )
                                                       );

            int spaceLen = m_Width - str.Length;
            Write( str + ( spaceLen > 0 ? new string( ' ', spaceLen ) : "" ) );

            UpdateCursorBlink( true );
        }

        private void UpdateCursorBlink( bool force )
        {
            // Since COMMON_LVB_UNDERSCORE doesn't work we won't be controlling blink
            //// Blink the cursor
            //if (Environment.TickCount > _inputCursorStateChange)
            //{
            //    _inputCursorStateChange = Environment.TickCount + 250;
            //    _inputCursorState = !_inputCursorState;
            //    force = true;
            //}
            //if (force)
            //    SetCursorBlink(_cursorBlinkLeft, _cursorBlinkTop, _inputCursorState);
            SetCursorBlink( m_Left + m_CursorBlinkLeft, m_Top + m_CursorBlinkTop, true );
        }

        #endregion

    }

}
