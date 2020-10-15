using RPG.GameObjects;
using RPG.UI;
using RPGEngine2;
using RPGEngine2.InputSystem;
using System;
using System.Collections.Generic;
using static RPGEngine2.EngineMain;

namespace RPG
{
    // TODO: maybe play around with bullet-time?
    internal class GameCode
    {
        public static readonly float MachineGunFireRate = 0.1f;//0.15f;
        public static readonly float RPGFireRate = 0.35f;//0.3f;
        public static Mouse Mouse;
        public static Keyboard Keyboard;
        public static Controller Controller;
        public static int PlayerControllerID = -1;
        public static Player PlayerObj;
        public static List<Enemy> Enemies = new List<Enemy>();
        public static int BombsAlive;
        public static Random rand = new Random();

        private static DebugString triggerValueDebugString;
        private static ControllerConnectedMessage popupMessage;
        private static readonly float movementSpeed = 19;
        private static float machinegunFiretimer;
        private static float rpgFiretimer;
        private static bool stoppedVibration = true;
        private static bool controllerDisconnectedPopup = true;

        public static void Main(string[] args)
        {
            OnStart += Start;
            OnUpdate += Update;
            OnFixedUpdate += FixedUpdate;
            OnFixedUpdate += MainMenu.Update;

            EngineStart();
        }

        public static void Start()
        {
            //triggerValueDebugString = new DebugString("", new Vector2(10, 10));
            Mouse = new Mouse();
            Keyboard = new Keyboard();
            Controller = new Controller();
            InputDeviceHandler.ActivateDevice(Mouse);
            InputDeviceHandler.ActivateDevice(Keyboard);
            InputDeviceHandler.ActivateDevice(Controller);

            popupMessage = new ControllerConnectedMessage();
            Instantiate(popupMessage);

            MainMenu.LoadMenu();
            MainMenu.isAnimating = true;
        }

        public static void FixedUpdate()
        {
            if (PlayerControllerID == -1 && Controller.TryGetUnassignedController(out int id))
            {
                PlayerControllerID = id;
                Controller.DefaultControllerID = id;
            }
            if (!controllerDisconnectedPopup && !Controller.isControllerConnected(PlayerControllerID))
            {
                popupMessage.Popup("Controller disconnected...");
                controllerDisconnectedPopup = true;
            }
            if (controllerDisconnectedPopup && Controller.isControllerConnected(PlayerControllerID))
            {
                popupMessage.Popup("Controller connected...");
                controllerDisconnectedPopup = false;
            }

            //triggerValueDebugString.Message = Controller.TriggerValue(Controller.Trigger.Right, PlayerControllerID);

            if (MainMenu.MenuShown || PlayerObj is null)
                return;

            // Movement
            PlayerObj.Position += InputAxis.GetAxisVector() * FixedDeltaTime * movementSpeed * Vector2.ScreenRatio;

            // Spawn enemies
            if (Keyboard.ButtonDown(Keyboard.Key.P) || Controller.ButtonDown(Controller.Button.Y, PlayerControllerID))
            {
                Vector2 spawn = new Vector2(rand.Next(Console.WindowWidth), rand.Next(Console.WindowHeight));

                Progressbar newHealthbar = new Progressbar(6, '#', '\0');
                Enemy newEnemy = new Enemy(newHealthbar, spawn);
                Instantiate(newHealthbar);
                Instantiate(newEnemy);
                Enemies.Add(newEnemy);
            }

            // Shooting
            Vector2 shootingDirection;

            if (Controller.isControllerConnected(PlayerControllerID))
            {
                shootingDirection = Controller.ThumbstickValues(Controller.Thumbstick.Right, PlayerControllerID);
                if (shootingDirection == Vector2.Zero)
                {
                    shootingDirection = Controller.ThumbstickValues(Controller.Thumbstick.Left, PlayerControllerID);
                }
            }
            else
            {
                shootingDirection = (Mouse.Position - PlayerObj.Position).Normalize();
            }

            PlayerObj.LookingDirection = shootingDirection;


            if (machinegunFiretimer > 0)
                machinegunFiretimer -= FixedDeltaTime;
            if (rpgFiretimer > 0)
                rpgFiretimer -= FixedDeltaTime;

            if (rpgFiretimer <= 0 && (Mouse.ButtonDown(0) || Controller.ButtonDown(Controller.Button.RightShoulder, PlayerControllerID)))
            {
                Instantiate(new Rocket(PlayerObj.Position + Vector2.One, shootingDirection * 12, 2));
                rpgFiretimer += RPGFireRate;
                BombsAlive++;
                stoppedVibration = false;
            }

            if (machinegunFiretimer <= 0 && (Mouse.ButtonDown(1) || Controller.TriggerValue(Controller.Trigger.Right, PlayerControllerID) > 0))
            {
                Instantiate(new MachineGunBullet(PlayerObj.Position + Vector2.One, shootingDirection * 25));
                machinegunFiretimer += MachineGunFireRate;
            }
        }

        public static void Update()
        {
            if (MainMenu.MenuShown || PlayerObj is null)
            {
                if (Controller.ButtonPressed(Controller.Button.A, PlayerControllerID))
                {
                    MainMenu.StartGame();
                }

                return;
            }

            if (Keyboard.ButtonPressed(Keyboard.Key.Escape) || Controller.ButtonPressed(Controller.Button.B, PlayerControllerID))
            {
                MainMenu.EnableMenu();
            }

            if (BombsAlive == 0 && stoppedVibration == false)
            {
                Controller.StopVibration(PlayerControllerID);
                stoppedVibration = true;
            }
        }
    }
}
