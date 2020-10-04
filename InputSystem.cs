//using ConsoleInput;
using System;
using System.Runtime.InteropServices;

namespace RPGEngine2
{
    public static class InputSystem
    {
        private const int STD_INPUT_HANDLE = -10;
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        private const uint ENABLE_MOUSE_INPUT = 0x0010;

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
        internal enum MouseEventFlags // https://docs.microsoft.com/en-us/windows/console/mouse-event-record-str
        {
            DOUBLE_CLICK = 0x0002,
            MOUSE_MOVED = 0x0001,
            MOUSE_HWHEELED = 0x0008,
            MOUSE_WHEELED = 0x0004
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
        internal enum InputEventType
        {
            FOCUS_EVENT = 0x0010,
            KEY_EVENT = 0x0001,
            MENU_EVENT = 0x0008,
            MOUSE_EVENT = 0x0002,
            WINDOW_BUFFER_SIZE_EVENT = 0x0004
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT_RECORD
        {
            public InputEventType EventType;
            public INPUT_RECORD_UNION Event;
        };

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetNumberOfConsoleInputEvents(IntPtr hConsoleInput, out uint lpcNumberOfEvents);
        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
        private static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsRead);
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WriteConsoleInput(IntPtr hConsoleInput, INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsWritten);

        private static IntPtr StdInHandle;


        public static void Setup(bool quickSelect = false)
        {
            StdInHandle = GetStdHandle(STD_INPUT_HANDLE);

            SetConsoleMode(StdInHandle, (quickSelect ? 0 : ENABLE_EXTENDED_FLAGS) | ENABLE_MOUSE_INPUT);
        }

        public static void Refresh()
        {
            INPUT_RECORD[] inputBuffer = new INPUT_RECORD[32];
            GetNumberOfConsoleInputEvents(StdInHandle, out uint numberOfEvents);
            if (numberOfEvents > 0)
            {
                ReadConsoleInput(StdInHandle, inputBuffer, numberOfEvents, out numberOfEvents);
            }

            INPUT_RECORD[] otherInputBuffer = new INPUT_RECORD[numberOfEvents];
            uint otherInputCount = 0;

            for (int i = 0; i < numberOfEvents; i++)
            {
                switch (inputBuffer[i].EventType)
                {
                    case InputEventType.MOUSE_EVENT:
                        Mouse.HandleMouseEvent(inputBuffer[i].Event.MouseEvent);
                        break;
                    default:
                        otherInputBuffer[otherInputCount++] = inputBuffer[i];
                        break;
                }
            }

            WriteConsoleInput(StdInHandle, otherInputBuffer, otherInputCount, out uint _);

            Mouse.Update();
        }

        public static class Mouse
        {
            public const int MOUSE_BUTTON_COUNT = 5;

            public static Vector2 Position => new Vector2(x, y);

            public readonly static bool[] MouseDown = new bool[MOUSE_BUTTON_COUNT];
            public readonly static bool[] MouseUp = new bool[MOUSE_BUTTON_COUNT];
            public readonly static bool[] MousePress = new bool[MOUSE_BUTTON_COUNT];
            private readonly static bool[] mouseDownPrevious = new bool[MOUSE_BUTTON_COUNT];
            private readonly static bool[] mouseDownCurrent = new bool[MOUSE_BUTTON_COUNT];
            public static short x = 0, y = 0;

            public static bool ButtonUp(int buttonIndex) => MouseUp[buttonIndex];
            public static bool ButtonHeld(int buttonIndex) => MouseDown[buttonIndex];
            public static bool ButtonPressed(int buttonIndex) => MousePress[buttonIndex];

            internal static void Update()
            {
                // mousebutton held down
                for (int i = 0; i < MOUSE_BUTTON_COUNT; i++)
                {
                    MouseUp[i] = false;
                    MousePress[i] = false;


                    if (mouseDownCurrent[i] != mouseDownPrevious[i])
                    {
                        if (mouseDownCurrent[i])
                        {
                            MousePress[i] = true;
                            MouseDown[i] = true;
                        }
                        else
                        {
                            MouseUp[i] = true;
                            MouseDown[i] = false;
                        }
                    }

                    mouseDownPrevious[i] = mouseDownCurrent[i];
                }
            }

            internal static void HandleMouseEvent(MOUSE_EVENT_RECORD mouseEvent)
            {
                switch (mouseEvent.dwEventFlags)
                {
                    case MouseEventFlags.DOUBLE_CLICK:
                        break;
                    case MouseEventFlags.MOUSE_MOVED:
                        x = mouseEvent.dwMousePosition.X;
                        y = mouseEvent.dwMousePosition.Y;
                        break;
                    case MouseEventFlags.MOUSE_HWHEELED:
                        break;
                    case MouseEventFlags.MOUSE_WHEELED:
                        break;
                    case 0:
                        // mousebutton pressed or up
                        for (int n = 0; n < MOUSE_BUTTON_COUNT; n++)
                        {
                            mouseDownCurrent[n] = (mouseEvent.dwButtonState & (1 << n)) != 0;
                        }
                        break;
                }
            }
        }

        public static void Initialize()
        {
            Setup(false);
        }
    }
}
