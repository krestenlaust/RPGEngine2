using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGEngine2;
using RPGEngine2.InputSystem;
using static RPGEngine2.EngineMain;

namespace BasicRPG
{
    class Program
    {
        static Keyboard Keyboard;
        static Mouse Mouse;

        static void Main(string[] args)
        {
            OnStart += Start;

            EngineStart();
        }

        static void Start()
        {
            Keyboard = new Keyboard();
            Mouse = new Mouse();

            InputDeviceHandler.ActivateDevice(Keyboard);
            InputDeviceHandler.ActivateDevice(Mouse);
        }
    }
}
