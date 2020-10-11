using RPGEngine2;
using RPGEngine2.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGame2.InputSystem
{
    public enum Axis
    {
        Horizontal,
        Vertical
    }

    public static class InputAxis
    {
        private static float previousHorizontalAxis;
        private static float currentHorizontalAxis;
        private static float previousVerticalAxis;
        private static float currentVerticalAxis;
        private static bool checkedAxisThisFrame;

        /// <summary>
        /// Returns value between 1 and -1 (inclusive). No dampening.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float GetAxisAnalog(Axis axis)
        {
            if (!checkedAxisThisFrame)
            {
                foreach (var axisDevice in InputDeviceHandler.GetAxisDevices())
                {
                    currentHorizontalAxis += axisDevice.GetAxisRaw(Axis.Horizontal);
                    currentVerticalAxis += axisDevice.GetAxisRaw(Axis.Vertical);
                }
                checkedAxisThisFrame = true;
            }

            switch (axis)
            {
                case Axis.Horizontal:
                    return Math.Max(Math.Min(currentHorizontalAxis, 1), -1);
                case Axis.Vertical:
                    return Math.Max(Math.Min(currentVerticalAxis, 1), -1);
            }

            return 0;
        }

        public static Vector2 GetAxisVector() => new Vector2(GetAxisAnalog(Axis.Horizontal), GetAxisAnalog(Axis.Vertical));


        internal static void Update()
        {
            previousHorizontalAxis = currentHorizontalAxis;
            previousVerticalAxis = currentVerticalAxis;
            currentHorizontalAxis = 0;
            currentVerticalAxis = 0;
            checkedAxisThisFrame = false;
        }
    }
}
