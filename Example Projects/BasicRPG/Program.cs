using BasicRPG.UI;
using RPGEngine2;
using RPGEngine2.InputSystem;
using static RPGEngine2.EngineMain;

namespace BasicRPG
{
    class Program
    {
        static Keyboard Keyboard;
        static Mouse Mouse;
        static Controller Controller;
        static BasicTextbox outputText;
        static int playerController = -1;

        static void Main(string[] args)
        {
            OnStart += Start;
            OnUpdate += Update;

            EngineStart();
        }

        static void Start()
        {
            Keyboard = new Keyboard();
            Mouse = new Mouse();
            Controller = new Controller();

            InputDeviceHandler.ActivateDevice(Keyboard);
            InputDeviceHandler.ActivateDevice(Mouse);
            InputDeviceHandler.ActivateDevice(Controller);

            outputText = new BasicTextbox(new Vector2(5, 5), new Vector2(25, 1), "Hello world");
            Instantiate(outputText);
        }

        static void Update()
        {
            if (Controller.TryGetUnassignedController(out int id))
            {
                playerController = id;
                outputText.Text = "Controller connected!";
            }

            if (playerController != -1)
            {
                outputText.Text = Controller.ThumbstickValues(Controller.Thumbstick.Left, playerController).ToString();
                //outputText.Text = Controller.TriggerValue(Controller.Trigger.Right, playerController).ToString();
            }
        }
    }
}
