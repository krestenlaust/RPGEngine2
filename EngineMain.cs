using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace RPGEngine2
{
    public static class EngineMain
    {
        public const int FPS_CAP = 60;
        private const int TIME_PER_FRAME = 1000 / FPS_CAP;
        public static float DeltaTime { get; private set; } = 1;
        
        //private static char[] ScreenBufferGame;
        //private static char[] ScreenBufferUI;
        private static readonly List<UIElementBase> UIElements = new List<UIElementBase>();
        private static readonly List<GameObjectBase> GameObjects = new List<GameObjectBase>();
        private static readonly List<UIElementBase> InstantiatedUIElements = new List<UIElementBase>();
        private static readonly List<GameObjectBase> InstantiatedGameObjects = new List<GameObjectBase>();
        private static bool isRunning;

        public static event Action OnStart;
        public static event Action OnUpdate;

        public static void Instantiate(GameObjectBase gameObject) => InstantiatedGameObjects.Add(gameObject);
        public static void Instantiate(UIElementBase element) => InstantiatedUIElements.Add(element);
        public static void EngineStop() => isRunning = false;

        public static void EngineStart()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Renderer.ScreenHeight = (short)(Console.WindowHeight - 1);
            Renderer.ScreenWidth = (short)(Console.WindowWidth);
            
            Console.CursorVisible = false;

            isRunning = true;
            OnStart?.Invoke();

            while (isRunning)
            {
                // Cycles & Time
                DeltaTime = sw.ElapsedMilliseconds / 1000f;
                Console.Title = "FPS: " + 1f / DeltaTime;
                sw.Restart();

                // Input
                InputSystem.Refresh();

                // Logic
                OnUpdate?.Invoke();
                UpdateUIElements(UIElements);
                UpdateGameObjects(GameObjects);

                // Render & draw
                Renderer.ResetScreenBuffers();
                Renderer.PerformRendering(UIElements, GameObjects);
                Renderer.WriteStandardOut();

                // High-level garbage collection
                UIElements.RemoveAll(element => element is null || element.isDestroyed);
                GameObjects.RemoveAll(gameObject => gameObject is null || gameObject.isDestroyed);

                // Spawn instantiated objects
                UIElements.AddRange(InstantiatedUIElements);
                GameObjects.AddRange(InstantiatedGameObjects);
                InstantiatedUIElements.Clear();
                InstantiatedGameObjects.Clear();

#pragma warning disable CS0162 // Unreachable code detected
                if (FPS_CAP == 0)
                    continue;
#pragma warning restore CS0162 // Unreachable code detected

                while (sw.ElapsedMilliseconds < TIME_PER_FRAME)
                    Thread.Sleep(1);
            }
        }

        
        private static void UpdateGameObjects(List<GameObjectBase> gameObjects)
        {
            foreach (GameObjectBase item in gameObjects)
            {
                if (item is null || !item.Active)
                    continue;

                item.Update();
                item.InternalPosition += item.Velocity * DeltaTime;

                item.Render();
            }
        }

        private static void UpdateUIElements(List<UIElementBase> elements)
        {
            foreach (UIElementBase item in elements)
            {
                if (item is null || !item.Active)
                    continue;

                bool hoverFrame = item.InsideBounds(new Vector2(InputSystem.Mouse.x, InputSystem.Mouse.y));
                if (hoverFrame)
                {
                    if (item.Hovered)
                    {
                        item.HoverStay();
                    }
                    else
                    {
                        item.HoverEnter();
                    }
                }
                else if (item.Hovered)
                {
                    item.HoverLeave();
                }
                item.Hovered = hoverFrame;

                item.Update();
                item.Render();
            }
        }

        

    }
}
