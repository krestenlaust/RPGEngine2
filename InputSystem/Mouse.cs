using System;

namespace RPGEngine2.InputSystem
{
    public class Mouse : IInputDevice
    {
        public const int MOUSE_BUTTON_COUNT = 5;
        private readonly bool[] mouseDown = new bool[MOUSE_BUTTON_COUNT];
        private readonly bool[] mouseUp = new bool[MOUSE_BUTTON_COUNT];
        private readonly bool[] mousePress = new bool[MOUSE_BUTTON_COUNT];

        public short x { get; private set; }
        public short y { get; private set; }

        private readonly bool[] mouseDownPrevious = new bool[MOUSE_BUTTON_COUNT];
        private readonly bool[] mouseDownCurrent = new bool[MOUSE_BUTTON_COUNT];

        public Mouse()
        {
        }

        /// <summary>
        /// Compensates for screen position.
        /// </summary>
        public Vector2 Position => new Vector2(x, y) + new Vector2(Console.WindowLeft, Console.WindowTop);
        public bool ButtonReleased(int buttonIndex) => mouseUp[buttonIndex];
        public bool ButtonDown(int buttonIndex) => mouseDown[buttonIndex];
        public bool ButtonPressed(int buttonIndex) => mousePress[buttonIndex];

        public void Initialize()
        {
            NativeMethods.StdInHandle = NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE);

            NativeMethods.SetConsoleMode(NativeMethods.StdInHandle, NativeMethods.ENABLE_EXTENDED_FLAGS | NativeMethods.ENABLE_MOUSE_INPUT);
        }

        public void Update()
        {
            NativeMethods.GetNumberOfConsoleInputEvents(NativeMethods.StdInHandle, out uint numberOfEvents);
            NativeMethods.INPUT_RECORD[] inputBuffer = new NativeMethods.INPUT_RECORD[numberOfEvents];

            if (numberOfEvents > 0)
            {
                NativeMethods.ReadConsoleInput(NativeMethods.StdInHandle, inputBuffer, numberOfEvents, out numberOfEvents);
            }

            NativeMethods.INPUT_RECORD[] otherInputBuffer = new NativeMethods.INPUT_RECORD[numberOfEvents];
            uint otherInputCount = 0;

            for (int i = 0; i < numberOfEvents; i++)
            {
                switch (inputBuffer[i].EventType)
                {
                    case NativeMethods.InputEventType.MOUSE_EVENT:
                        HandleMouseEvent(inputBuffer[i].Event.MouseEvent);
                        break;
                    default:
                        otherInputBuffer[otherInputCount++] = inputBuffer[i];
                        break;
                }
            }

            if (NativeMethods.REWRITE_UNUSED_INPUT)
            {
                NativeMethods.WriteConsoleInput(NativeMethods.StdInHandle, otherInputBuffer, otherInputCount, out uint _);
            }


            // mousebutton held down
            for (int i = 0; i < MOUSE_BUTTON_COUNT; i++)
            {
                mouseUp[i] = false;
                mousePress[i] = false;


                if (mouseDownCurrent[i] != mouseDownPrevious[i])
                {
                    if (mouseDownCurrent[i])
                    {
                        mousePress[i] = true;
                        mouseDown[i] = true;
                    }
                    else
                    {
                        mouseUp[i] = true;
                        mouseDown[i] = false;
                    }
                }

                mouseDownPrevious[i] = mouseDownCurrent[i];
            }
        }

        private void HandleMouseEvent(NativeMethods.MOUSE_EVENT_RECORD mouseEvent)
        {
            switch (mouseEvent.dwEventFlags)
            {
                case NativeMethods.MouseEventFlags.DOUBLE_CLICK:
                    break;
                case NativeMethods.MouseEventFlags.MOUSE_MOVED:
                    x = mouseEvent.dwMousePosition.X;
                    y = mouseEvent.dwMousePosition.Y;
                    break;
                case NativeMethods.MouseEventFlags.MOUSE_HWHEELED:
                    break;
                case NativeMethods.MouseEventFlags.MOUSE_WHEELED:
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
}
