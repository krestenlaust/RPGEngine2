using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGEngine2.InputSystem
{
    public static class InputDeviceHandler
    {
        private static readonly List<IInputDevice> inputDevices = new List<IInputDevice>();
        internal static Mouse InternalMouseDevice = null;

        public static void RefreshDevices()
        {
            foreach (var device in inputDevices)
            {
                device.Update();
            }
        }

        public static void ActivateDevice(IInputDevice device)
        {
            if (inputDevices.Contains(device))
            {
                throw new DeviceAlreadyExistsException();
            }

            device.Initialize();
            inputDevices.Add(device);

            if (device is Mouse mouse)
            {
                InternalMouseDevice = mouse;
            }
        }

        public static T GetDevice<T>()
        {
            return inputDevices.OfType<T>().First();
        }

        public class DeviceAlreadyExistsException : Exception {
            public DeviceAlreadyExistsException()
            {
            }
        }
    }
}
