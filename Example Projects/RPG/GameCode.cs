using CommonComponents.UI;
using RPG.GameObjects;
using RPGEngine2;
using RPGEngine2.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using static RPGEngine2.EngineMain;

namespace RPG
{
    // TODO: maybe play around with bullet-time?
    internal class GameCode
    {
        public static readonly int MaxEnemies = 20;
        public static readonly float SpawnInterval = 0.25f;
        public static readonly float MachineGunFireRate = 0.05f;//0.15f;
        public static readonly float RocketFireRate = 0.15f;//0.3f;
        public static Mouse Mouse;
        public static Keyboard Keyboard;
        public static Controller Controller;
        public static int PlayerControllerID = -1;
        public static Player PlayerObj;
        public static List<Enemy> Enemies = new List<Enemy>();
        public static int BombsAlive;
        public static Random rand = new Random();
        public static float spawnTimer;
        public static int EnemyCount;

        private static ControllerStatusMessage popupMessage;
        private static readonly float movementSpeed = 19;
        private static float machinegunFiretimer;
        private static float rocketFiretimer;
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
            Mouse = new Mouse();
            Keyboard = new Keyboard();
            Controller = new Controller();
            InputDeviceHandler.ActivateDevice(Mouse);
            InputDeviceHandler.ActivateDevice(Keyboard);
            InputDeviceHandler.ActivateDevice(Controller);

            popupMessage = new ControllerStatusMessage();
            Instantiate(popupMessage);

            MainMenu.LoadMenu();
            MainMenu.isAnimating = true;

            //Renderer.VoidfillChar = '.';
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

            // Ignore game logic if menu is open.
            if (MainMenu.MenuShown || PlayerObj is null)
            {
                return;
            }

            if (PlayerObj.HP < 0)
            {
                EngineStop();
            }

            // Movement
            PlayerObj.Position += InputAxis.GetAxisVector() * FixedDeltaTime * movementSpeed * Vector2.ScreenRatio;

            spawnTimer -= FixedDeltaTime;

            if (EnemyCount < MaxEnemies)
            {
                // Spawn enemies
                if (Keyboard.ButtonDown(Keyboard.Key.P) || Controller.ButtonDown(Controller.Button.Y, PlayerControllerID))
                {
                    SpawnEnemy();
                }

                if (spawnTimer < 0)
                {
                    SpawnEnemy();
                    spawnTimer = SpawnInterval;
                }
            }

            // Shooting
            Vector2 shootingDirection;

            if (Controller.isControllerConnected(PlayerControllerID))
            {
                // Get shooting direction from joystick.
                shootingDirection = Controller.ThumbstickValues(Controller.Thumbstick.Right, PlayerControllerID);

                if (shootingDirection == Vector2.Zero)
                {
                    shootingDirection = Controller.ThumbstickValues(Controller.Thumbstick.Left, PlayerControllerID);
                }
            }
            else
            {
                // Get shooting direction from mouse position.
                shootingDirection = (Mouse.Position - PlayerObj.Position).Normalize();
            }

            PlayerObj.LookingDirection = shootingDirection;

            if (machinegunFiretimer > 0)
            {
                machinegunFiretimer -= FixedDeltaTime;
            }

            if (rocketFiretimer > 0)
            {
                rocketFiretimer -= FixedDeltaTime;
            }

            if (rocketFiretimer <= 0 && (Mouse.ButtonDown(0) || Controller.ButtonDown(Controller.Button.RightShoulder, PlayerControllerID)))
            {
                Instantiate(new Rocket(PlayerObj.Position + Vector2.One, shootingDirection * 12, 2));
                rocketFiretimer += RocketFireRate;
                BombsAlive++;
                stoppedVibration = false;
            }

            if (machinegunFiretimer <= 0 && (Mouse.ButtonDown(1) || Controller.TriggerValue(Controller.Trigger.Right, PlayerControllerID) > 0))
            {
                Instantiate(new MachineGunBullet(PlayerObj.Position + Vector2.One, shootingDirection * 25));
                machinegunFiretimer += MachineGunFireRate;
            }
        }

        private static void SpawnEnemy()
        {
            Vector2 spawn = new Vector2(rand.Next(Console.WindowWidth), rand.Next(Console.WindowHeight));

            Progressbar newHealthbar = new Progressbar(6, '#', '\0');
            Enemy newEnemy = new Enemy(newHealthbar, spawn);
            Instantiate(newHealthbar);
            Instantiate(newEnemy);

            EnemyCount++;
            Enemies.Add(newEnemy);
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
