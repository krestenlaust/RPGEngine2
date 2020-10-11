using RPG.GameObjects;
using RPGEngine2;
using RPGEngine2.InputSystem;
using RPGGame2.InputSystem;
using System;
using System.Collections.Generic;
using static RPGEngine2.EngineMain;

namespace RPG
{
    // TODO: maybe play around with bullet-time?
    internal class GameCode
    {
        public static readonly float MachineGunFireRate = 0.15f;
        public static readonly float RPGFireRate = 0.3f;
        public static Mouse Mouse;
        public static Keyboard Keyboard;
        public static Controller Controller;
        public static int playerControllerID = -1;
        public static Player PlayerObj;
        public static List<Enemy> Enemies = new List<Enemy>();
        
        private static readonly float movementSpeed = 19;
        private static float machinegunFiretimer = MachineGunFireRate;
        private static float rpgFiretimer = RPGFireRate;

        public static void Main(string[] args)
        {
            OnStart += Start;
            OnUpdate += Update;
            OnFixedUpdate += FixedUpdate;

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

            MainMenu.LoadMenu();
            MainMenu.isAnimating = true;
        }

        public static void FixedUpdate()
        {
            if (Controller.TryGetUnassignedController(out int id))
            {
                playerControllerID = id;
                Controller.DefaultControllerID = id;
            }

            if (MainMenu.MenuShown || PlayerObj is null)
                return;

            // Movement
            PlayerObj.Position += InputAxis.GetAxisVector() * FixedDeltaTime * movementSpeed * new Vector2(2, 1);

            // Spawn enemies
            if (Keyboard.ButtonDown(Keyboard.Key.P) || Controller.ButtonDown(Controller.Button.Y, playerControllerID))
            {
                Random rand = new Random();

                Vector2 spawn = new Vector2(rand.Next(Console.WindowWidth), rand.Next(Console.WindowHeight));

                Progressbar newHealthbar = new Progressbar(6, '#', '\0');
                Enemy newEnemy = new Enemy(newHealthbar, spawn);
                Instantiate(newHealthbar);
                Instantiate(newEnemy);
                Enemies.Add(newEnemy);
            }

            // Shooting
            Vector2 shootingDirection;

            if (Controller.isControllerConnected(playerControllerID))
            {
                shootingDirection = Controller.ThumbstickValues(Controller.Thumbstick.Right, playerControllerID);
                if (shootingDirection == Vector2.Zero)
                {
                    shootingDirection = Controller.ThumbstickValues(Controller.Thumbstick.Left, playerControllerID);
                }
            }
            else
            {
                shootingDirection = (Mouse.Position - PlayerObj.Position).Normalize();
            }

            PlayerObj.LookingDirection = shootingDirection;

            rpgFiretimer += FixedDeltaTime;
            if (Mouse.ButtonDown(0) || Keyboard.ButtonDown(Keyboard.Key.B) || 
                Controller.ButtonDown(Controller.Button.RightShoulder, playerControllerID) && rpgFiretimer >= RPGFireRate)
            {
                Instantiate(new Rocket(PlayerObj.Position + Vector2.One, shootingDirection * 10, 2));
                rpgFiretimer -= RPGFireRate;
            }

            machinegunFiretimer += FixedDeltaTime;
            if (Mouse.ButtonDown(1) || Keyboard.ButtonDown(Keyboard.Key.Space) ||
                Controller.TriggerValue(Controller.Trigger.Right, playerControllerID) > 0.1f &&
                machinegunFiretimer >= MachineGunFireRate)
            {
                Instantiate(new MachineGunBullet(PlayerObj.Position + Vector2.One, shootingDirection * 25));
                machinegunFiretimer -= MachineGunFireRate;
            }
        }

        public static void Update()
        {
            MainMenu.UpdateAnimation();

            if (MainMenu.MenuShown || PlayerObj is null)
            {
                if (Controller.ButtonPressed(Controller.Button.A, playerControllerID))
                {
                    MainMenu.StartGame();
                }
                
                return;
            }

            if (Keyboard.ButtonPressed(Keyboard.Key.Escape) || Controller.ButtonPressed(Controller.Button.B, playerControllerID))
            {
                MainMenu.EnableMenu();
            }
        }
    }
}
