using System;
using System.Runtime.InteropServices;
using System.Security;

namespace RPGEngine2
{
    // Is this secure?
    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        public const int STD_INPUT_HANDLE = -10;
        public const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        public const uint ENABLE_MOUSE_INPUT = 0x0010;
        public const bool REWRITE_UNUSED_INPUT = false;
        public static IntPtr StdInHandle;

        internal enum MouseEventFlags // https://docs.microsoft.com/en-us/windows/console/mouse-event-record-str
        {
            DOUBLE_CLICK = 0x0002,
            MOUSE_MOVED = 0x0001,
            MOUSE_HWHEELED = 0x0008,
            MOUSE_WHEELED = 0x0004
        }
        internal enum InputEventType
        {
            FOCUS_EVENT = 0x0010,
            KEY_EVENT = 0x0001,
            MENU_EVENT = 0x0008,
            MOUSE_EVENT = 0x0002,
            WINDOW_BUFFER_SIZE_EVENT = 0x0004
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FOCUS_EVENT_RECORD
        {
            public uint bSetFocus;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct MENU_EVENT_RECORD
        {
            public uint dwCommandId;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            public short X;
            public short Y;

            public COORD(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };
        internal struct WINDOW_BUFFER_SIZE_RECORD
        {
            public COORD dwSize;

            public WINDOW_BUFFER_SIZE_RECORD(short x, short y)
            {
                dwSize = new COORD(x, y);
            }
        }
        [StructLayout(LayoutKind.Explicit)]
        internal struct MOUSE_EVENT_RECORD
        {
            [FieldOffset(0)]
            public COORD dwMousePosition;
            [FieldOffset(4)]
            public uint dwButtonState;
            [FieldOffset(8)]
            public uint dwControlKeyState;
            [FieldOffset(12)]
            public MouseEventFlags dwEventFlags;
        }
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        internal struct KEY_EVENT_RECORD
        {
            [FieldOffset(0), MarshalAs(UnmanagedType.Bool)]
            public bool bKeyDown;
            [FieldOffset(4), MarshalAs(UnmanagedType.U2)]
            public ushort wRepeatCount;
            [FieldOffset(6), MarshalAs(UnmanagedType.U2)]
            public short wVirtualKeyCode;
            [FieldOffset(8), MarshalAs(UnmanagedType.U2)]
            public ushort wVirtualScanCode;
            [FieldOffset(10)]
            public char UnicodeChar;
            [FieldOffset(12), MarshalAs(UnmanagedType.U4)]
            public uint dwControlKeyState;
        }
        [StructLayout(LayoutKind.Explicit)]
        internal struct INPUT_RECORD_UNION
        {
            [FieldOffset(0)]
            public KEY_EVENT_RECORD KeyEvent;
            [FieldOffset(0)]
            public MOUSE_EVENT_RECORD MouseEvent;
            [FieldOffset(0)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
            [FieldOffset(0)]
            public MENU_EVENT_RECORD MenuEvent;
            [FieldOffset(0)]
            public FOCUS_EVENT_RECORD FocusEvent;
        };
        [StructLayout(LayoutKind.Sequential)]
        internal struct INPUT_RECORD
        {
            public InputEventType EventType;
            public INPUT_RECORD_UNION Event;
        };

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetNumberOfConsoleInputEvents(IntPtr hConsoleInput, out uint lpcNumberOfEvents);
        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
        internal static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsRead);
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool WriteConsoleInput(IntPtr hConsoleInput, INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsWritten);

        // Keyboard
        [DllImport("user32.dll")]
        internal static extern short GetKeyState(int nVirtKey);

        // Console
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // Edit sizeconst if the font name is too big
            public string FaceName;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref CONSOLE_FONT_INFO_EX fontExInfo);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref CONSOLE_FONT_INFO_EX fontExInfo);
    }
}
