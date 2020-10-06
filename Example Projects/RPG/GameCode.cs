﻿using RPG.GameObjects;
using RPGEngine2;
using static RPGEngine2.EngineMain;
using System.Collections.Generic;
using RPGEngine2.InputSystem;
using RPGGame2.InputSystem;

namespace RPG
{
    internal class GameCode
    {
        public static Mouse Mouse;
        public static Keyboard Keyboard;

        public static Player PlayerObj;
        public static List<Enemy> Enemies = new List<Enemy>();
        public static readonly float FireRate = 0.18f;
        private static readonly float movementSpeed = 19;
        private static float firetimer = FireRate;

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
            InputDeviceHandler.ActivateDevice(Mouse);
            InputDeviceHandler.ActivateDevice(Keyboard);

            //UIElements.Add(new AnimatedTextBox(new Vector2(10, 10), new Vector2(9, 4), "Start"));
            MainMenu.LoadMenu();
            MainMenu.isAnimating = true;
        }

        public static void Update()
        {
            MainMenu.UpdateAnimation();

            if (PlayerObj is null)
                return;

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

            if (Keyboard.ButtonPressed(Keyboard.Key.Space))
            {
                Progressbar newHealthbar = new Progressbar(6, '#', '\0');
            }
            /*
            while (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        PlayerObj.Position += Vector2.Left * 2;
                        break;
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        PlayerObj.Position += Vector2.Up;
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        PlayerObj.Position += Vector2.Right * 2;
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        PlayerObj.Position += Vector2.Down;
                        break;
                    case ConsoleKey.Spacebar:
                        Progressbar newHealthbar = new Progressbar(6, '#', '\0');
                        Enemy newEnemy = new Enemy(newHealthbar, new Vector2(40, 10));
                        Instantiate(newHealthbar);
                        Instantiate(newEnemy);
                        Enemies.Add(newEnemy);
                        break;
                }
            }*/

            if (Mouse.ButtonPressed(0))
            {
                Vector2 velocity = (Mouse.Position - PlayerObj.Position).Normalize() * 10;
                Instantiate(new Rocket(PlayerObj.Position, velocity, 2));
            }

            firetimer += DeltaTime;
            if (Mouse.ButtonDown(1) && firetimer >= FireRate)
            {
                Vector2 velocity = Mouse.Position - PlayerObj.Position;
                //double angleSpread = rand.NextDouble() * 2;
                //velocity += new Vector2((float)Math.Cos(angleSpread), (float)Math.Sin(angleSpread));

                Instantiate(new MachineGunBullet(PlayerObj.Position, velocity.Normalize() * 20));
                firetimer = 0;
            }
        }
    }
}