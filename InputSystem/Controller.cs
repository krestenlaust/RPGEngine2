using RPGGame2.InputSystem;
using System.Collections.Generic;
using XInputDotNetPure;

namespace RPGEngine2.InputSystem
{
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

        public bool ButtonPressed(Button button, int controllerID)
        {
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
            if (isButtonDown(button, controllerID))
            {
                buttonDownCurrent.Add((button, controllerID));
                return true;
            }

            return false;
        }

        public void Initialize()
        {
        }

        public void Update()
        {
            controllerStates.Clear();

            buttonDownPrevious = buttonDownCurrent;
            buttonDownCurrent = new HashSet<(Button, int)>();
        }

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



        public float TriggerValue(Trigger trigger, int controllerID)
        {
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
            GamePadState gamepadState = GetGamePadState(controllerID);

            return thumbstick switch
            {
                Thumbstick.Left => new Vector2(gamepadState.ThumbSticks.Left.X, -gamepadState.ThumbSticks.Left.Y),
                Thumbstick.Right => new Vector2(gamepadState.ThumbSticks.Right.X, -gamepadState.ThumbSticks.Right.Y),
                _ => Vector2.Zero,
            };
        }

        public bool isControllerConnected(int controllerID) => GetGamePadState(controllerID).IsConnected;

        private void UpdateAssignedControllerList()
        {
            // Check if controller is assigned. Iterating backwards to prevent issues when removing entries.
            for (int i = assignedControllerIDs.Count - 1; i >= 0; i--)
            {
                if (!isControllerConnected(i))
                    assignedControllerIDs.Remove(i);
            }
        }

        public void StopVibration(int controllerID) => GamePad.SetVibration((PlayerIndex)controllerID, 0, 0);

        public void SetVibration(float leftMotor, float rightMotor, int controllerID) => GamePad.SetVibration((PlayerIndex)controllerID, leftMotor, rightMotor);

        /// <summary>
        /// Checks if an unregistered controller has been plugged in.
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

        /// <summary>
        /// Returns the movement of left-axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetAxisRaw(Axis axis)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return ThumbstickValues(Thumbstick.Left, DefaultControllerID).x;
                case Axis.Vertical:
                    return ThumbstickValues(Thumbstick.Left, DefaultControllerID).y;
                default:
                    return 0;
            }
        }
    }
}
