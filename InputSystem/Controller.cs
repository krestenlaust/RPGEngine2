using RPGEngine2.InputSystem;
using System;
using System.Collections.Generic;
using XInputDotNetPure;

namespace RPGEngine2.InputSystem
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Even though the XInput controller class is mostly static, I've chosen to keep methods non-static to follow IInputDevice design-principle.")]
    public class Controller : IInputDevice, IAxisInput
    {
        public enum Button
        {
            Start,
            Back,
            LeftStick,
            RightStick,
            LeftShoulder,
            RightShoulder,
            Guide,
            A,
            B,
            X,
            Y,
            DPadUp,
            DPadDown,
            DPadLeft,
            DPadRight
        }
        public enum Trigger
        {
            Left,
            Right,
        }
        public enum Thumbstick
        {
            Left,
            Right,
        }


        public const int MaxControllers = 4;

        /// <summary>
        /// Used for InputAxis.
        /// </summary>
        public int DefaultControllerID = -1;

        private readonly HashSet<int> assignedControllerIDs = new HashSet<int>();
        private readonly Dictionary<int, GamePadState> controllerStates = new Dictionary<int, GamePadState>(4);

        private HashSet<(Button, int)> buttonDownPrevious;
        private HashSet<(Button, int)> buttonDownCurrent = new HashSet<(Button, int)>();

        // Debouncing
        private struct VibrationDebounce
        {
            public DateTime TriggerTime;
            public float LeftMoter;
            public float RightMoter;

            public VibrationDebounce(float debounceDuration, float leftMoter, float rightMoter)
            {
                TriggerTime = DateTime.Now + TimeSpan.FromSeconds(debounceDuration);
                LeftMoter = leftMoter;
                RightMoter = rightMoter;
            }
        }
        private Dictionary<int, VibrationDebounce> vibration = new Dictionary<int, VibrationDebounce>(4);


        public void Initialize()
        {
        }

        public void Update()
        {
            controllerStates.Clear();

            buttonDownPrevious = buttonDownCurrent;
            buttonDownCurrent = new HashSet<(Button, int)>();


            Stack<int> removeEntries = new Stack<int>(vibration.Count);

            foreach (var item in vibration)
            {
                if (item.Value.TriggerTime > DateTime.Now)
                    continue;

                SetVibration(item.Value.LeftMoter, item.Value.RightMoter, item.Key);
                removeEntries.Push(item.Key);
            }

            while (removeEntries.Count > 0)
                vibration.Remove(removeEntries.Pop());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="debounceTime">Time till event will happen in seconds.</param>
        /// <param name="lowFrequencyRumble"></param>
        /// <param name="highFrequencyRumble"></param>
        public void SetVibrationDebounce(int controllerID, float debounceTime, float lowFrequencyRumble, float highFrequencyRumble)
        {
            vibration[controllerID] = new VibrationDebounce(debounceTime, lowFrequencyRumble, highFrequencyRumble);
        }

        public void StopVibration(int controllerID)
        {
            if (!isControllerIDValid(controllerID))
                return;

            GamePad.SetVibration((PlayerIndex)controllerID, 0, 0);
        }

        public void SetVibration(float lowFrequencyRumble, float highFrequencyRumble, int controllerID)
        {
            if (!isControllerIDValid(controllerID))
                return;

            GamePad.SetVibration((PlayerIndex)controllerID, lowFrequencyRumble, highFrequencyRumble);
        }

        public bool ButtonPressed(Button button, int controllerID)
        {
            if (!isControllerIDValid(controllerID))
                return false;

            if (isButtonDown(button, controllerID))
            {
                (Button, int) buttonPress = (button, controllerID);
                buttonDownCurrent.Add(buttonPress);

                if (!buttonDownPrevious.Contains(buttonPress))
                    return true;
            }

            return false;
        }

        public bool ButtonReleased(Button button, int controllerID)
        {
            if (!isControllerIDValid(controllerID))
                return false;

            (Button, int) buttonPress = (button, controllerID);

            if (isButtonDown(button, controllerID))
            {
                buttonDownCurrent.Add(buttonPress);
                return false;
            }

            return buttonDownPrevious.Contains(buttonPress);
        }

        public bool ButtonDown(Button button, int controllerID)
        {
            if (!isControllerIDValid(controllerID))
                return false;

            if (isButtonDown(button, controllerID))
            {
                buttonDownCurrent.Add((button, controllerID));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the movement of left-axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetAxisRaw(Axis axis)
        {
            return axis switch
            {
                Axis.Horizontal => ThumbstickValues(Thumbstick.Left, DefaultControllerID).x,
                Axis.Vertical => ThumbstickValues(Thumbstick.Left, DefaultControllerID).y,
                _ => 0,
            };
        }

        public float TriggerValue(Trigger trigger, int controllerID)
        {
            if (!isControllerIDValid(controllerID))
                return 0;

            GamePadState gamepadState = GetGamePadState(controllerID);

            return trigger switch
            {
                Trigger.Left => gamepadState.Triggers.Left,
                Trigger.Right => gamepadState.Triggers.Right,
                _ => 0,
            };
        }

        public Vector2 ThumbstickValues(Thumbstick thumbstick, int controllerID)
        {
            if (!isControllerIDValid(controllerID))
                return Vector2.Zero;

            GamePadState gamepadState = GetGamePadState(controllerID);

            return thumbstick switch
            {
                Thumbstick.Left => new Vector2(gamepadState.ThumbSticks.Left.X, -gamepadState.ThumbSticks.Left.Y),
                Thumbstick.Right => new Vector2(gamepadState.ThumbSticks.Right.X, -gamepadState.ThumbSticks.Right.Y),
                _ => Vector2.Zero,
            };
        }

        public bool isControllerConnected(int controllerID)
        {
            if (!isControllerIDValid(controllerID))
                return false;

            return GetGamePadState(controllerID).IsConnected;
        }

        /// <summary>
        /// Checks whether a new controller has been plugged in.
        /// </summary>
        /// <param name="ControllerID"></param>
        /// <returns></returns>
        public bool TryGetUnassignedController(out int ControllerID)
        {
            UpdateAssignedControllerList();

            for (int i = 0; i < MaxControllers; i++)
            {
                if (assignedControllerIDs.Contains(i))
                    continue;

                if (isControllerConnected(i))
                {
                    ControllerID = i;
                    assignedControllerIDs.Add(i);
                    return true;
                }
            }

            ControllerID = -1;
            return false;
        }

        private bool isButtonDown(Button button, int controllerID)
        {
            GamePadState gamepadState = GetGamePadState(controllerID);

            switch (button)
            {
                case Button.Start:
                    return gamepadState.Buttons.Start == ButtonState.Pressed;
                case Button.Back:
                    return gamepadState.Buttons.Back == ButtonState.Pressed;
                case Button.LeftStick:
                    return gamepadState.Buttons.LeftStick == ButtonState.Pressed;
                case Button.RightStick:
                    return gamepadState.Buttons.RightStick == ButtonState.Pressed;
                case Button.LeftShoulder:
                    return gamepadState.Buttons.LeftShoulder == ButtonState.Pressed;
                case Button.RightShoulder:
                    return gamepadState.Buttons.RightShoulder == ButtonState.Pressed;
                case Button.Guide:
                    return gamepadState.Buttons.Guide == ButtonState.Pressed;
                case Button.A:
                    return gamepadState.Buttons.A == ButtonState.Pressed;
                case Button.B:
                    return gamepadState.Buttons.B == ButtonState.Pressed;
                case Button.X:
                    return gamepadState.Buttons.X == ButtonState.Pressed;
                case Button.Y:
                    return gamepadState.Buttons.Y == ButtonState.Pressed;

                case Button.DPadUp:
                    return gamepadState.DPad.Up == ButtonState.Pressed;
                case Button.DPadDown:
                    return gamepadState.DPad.Down == ButtonState.Pressed;
                case Button.DPadLeft:
                    return gamepadState.DPad.Left == ButtonState.Pressed;
                case Button.DPadRight:
                    return gamepadState.DPad.Right == ButtonState.Pressed;

                default:
                    return false;
            }
        }

        private static bool isControllerIDValid(int controllerID) => !(controllerID < 0 || controllerID > 3);

        private GamePadState GetGamePadState(int id)
        {
            if (controllerStates.TryGetValue(id, out GamePadState state))
            {
                return state;
            }

            state = GamePad.GetState((PlayerIndex)id);
            controllerStates[id] = state;
            return state;
        }

        private void UpdateAssignedControllerList()
        {
            // Check whether a controller that is marked as assigned is still plugged in. Iterating backwards to prevent issues when removing entries.
            for (int i = assignedControllerIDs.Count - 1; i >= 0; i--)
            {
                if (!isControllerConnected(i))
                    assignedControllerIDs.Remove(i);
            }
        }
    }
}
