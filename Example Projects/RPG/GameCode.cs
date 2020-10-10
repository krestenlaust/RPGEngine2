using static RPGEngine2.EngineMain;
using RPG.GameObjects;
using RPGEngine2;
using System.Collections.Generic;
using RPGEngine2.InputSystem;
using System;

namespace RPG
{
    // TODO: maybe play around with bullet-time?
    internal class GameCode
    {
        public static Mouse Mouse;
        public static Keyboard Keyboard;
        public static Controller Controller;

        public static Player PlayerObj;
        public static List<Enemy> Enemies = new List<Enemy>();
        public static readonly float FireRate = 0.1f;
        private static readonly float movementSpeed = 19;
        private static float firetimer = FireRate;
        public static int controllerID = -1;

        public static void Main(string[] args)
        {
            OnStart += Start;
            OnUpdate += Update;

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

        public static void Update()
        {
            if (controllerID == -1 && Controller.TryGetUnassignedController(out int id))
            {
                controllerID = id;
            }

            MainMenu.UpdateAnimation();

            if (MainMenu.MenuShown || PlayerObj is null)
            {
                if (Controller.ButtonPressed(Controller.Button.A, controllerID))
                {
                    MainMenu.StartGame();
                }
                return;
            }

            Vector2 shootingDirection = Vector2.Zero;

            if (Controller.isControllerConnected(controllerID))
            {
                Vector2 moveDirection = Controller.ThumbstickValues(Controller.Thumbstick.Left, controllerID) * new Vector2(2, 1);
                PlayerObj.Position += moveDirection * DeltaTime * movementSpeed;

                shootingDirection = Controller.ThumbstickValues(Controller.Thumbstick.Right, controllerID);
                if (shootingDirection == Vector2.Zero)
                {
                    shootingDirection = Controller.ThumbstickValues(Controller.Thumbstick.Left, controllerID);
                }
            }
            else
            {
                if (Keyboard.ButtonDown(Keyboard.Key.A) || Keyboard.ButtonDown(Keyboard.Key.Left))
                {
                    PlayerObj.Position += Vector2.Left * 2 * DeltaTime * movementSpeed;
                }
                else if (Keyboard.ButtonDown(Keyboard.Key.D) || Keyboard.ButtonDown(Keyboard.Key.Right))
                {
                    PlayerObj.Position += Vector2.Right * 2 * DeltaTime * movementSpeed;
                }

                if (Keyboard.ButtonDown(Keyboard.Key.W) || Keyboard.ButtonDown(Keyboard.Key.Up))
                {
                    PlayerObj.Position += Vector2.Up * DeltaTime * movementSpeed;
                }
                else if (Keyboard.ButtonDown(Keyboard.Key.S) || Keyboard.ButtonDown(Keyboard.Key.Down))
                {
                    PlayerObj.Position += Vector2.Down * DeltaTime * movementSpeed;
                }

                shootingDirection = (Mouse.Position - PlayerObj.Position).Normalize();
            }

            PlayerObj.LookingDirection = shootingDirection;
            

            if (Keyboard.ButtonPressed(Keyboard.Key.P) || Controller.ButtonPressed(Controller.Button.Y, controllerID))
            {
                Random rand = new Random();

                Vector2 spawn = new Vector2(rand.Next(Console.WindowWidth), rand.Next(Console.WindowHeight));

                Progressbar newHealthbar = new Progressbar(6, '#', '\0');
                Enemy newEnemy = new Enemy(newHealthbar, spawn);
                Instantiate(newHealthbar);
                Instantiate(newEnemy);
                Enemies.Add(newEnemy);
            }

            if (Keyboard.ButtonPressed(Keyboard.Key.Escape))
            {
                MainMenu.EnableMenu();
            }

            if (Mouse.ButtonReleased(0) || Keyboard.ButtonDown(Keyboard.Key.B) || Controller.ButtonDown(Controller.Button.RightShoulder, controllerID))
            {
                Instantiate(new Rocket(PlayerObj.Position + Vector2.One, shootingDirection * 10, 2));
            }

            firetimer += DeltaTime;
            if (Mouse.ButtonDown(1) || 
                Keyboard.ButtonDown(Keyboard.Key.Space) || 
                Controller.TriggerValue(Controller.Trigger.Right, controllerID) > 0.1f && 
                firetimer >= FireRate)
            {
                Instantiate(new MachineGunBullet(PlayerObj.Position + Vector2.One, shootingDirection * 25));
                firetimer = 0;
            }
        }
    }
}
