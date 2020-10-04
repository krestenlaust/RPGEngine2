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
        
        private static readonly List<BaseObject> baseObjects = new List<BaseObject>();
        private static readonly List<BaseObject> instantiatedBaseObjects = new List<BaseObject>();
        private static bool isRunning;

        public static event Action OnStart;
        public static event Action OnUpdate;

        public static void Instantiate(BaseObject baseObject) => instantiatedBaseObjects.Add(baseObject);
        public static void EngineStop() => isRunning = false;

        public static void EngineStart()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Renderer.ScreenHeight = (short)(Console.WindowHeight - 1);
            Renderer.ScreenWidth = (short)(Console.WindowWidth);
            InputSystem.Initialize();

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
                UpdateBaseObject(baseObjects);

                // Render & draw
                Renderer.ResetScreenBuffers();
                Renderer.PerformRendering(baseObjects);
                Renderer.WriteStandardOut();

                // High-level garbage collection
                baseObjects.RemoveAll(gameObject => gameObject is null || gameObject.isDestroyed);

                // Spawn instantiated objects
                baseObjects.AddRange(instantiatedBaseObjects);
                instantiatedBaseObjects.Clear();

#pragma warning disable CS0162 // Unreachable code detected
                if (FPS_CAP == 0)
                    continue;
#pragma warning restore CS0162 // Unreachable code detected

                while (sw.ElapsedMilliseconds < TIME_PER_FRAME)
                    Thread.Sleep(1);
            }
        }


        private static void UpdateBaseObject(List<BaseObject> baseObjects)
        {
            foreach (BaseObject item in baseObjects)
            {
                if (item is null || !item.Active)
                    continue;

                switch (item)
                {
                    case GameObjectBase gameObject:
                        gameObject.Update();
                        gameObject.InternalPosition += gameObject.Velocity * DeltaTime;
                        break;

                    case UIElementBase uiElement:
                        bool hoverFrame = uiElement.InsideBounds(new Vector2(InputSystem.Mouse.x, InputSystem.Mouse.y));
                        UIElementBase.HoverState previousHoverState = uiElement.CurrentHoverState;

                        if (hoverFrame)
                        {
                            if (uiElement.CurrentHoverState == UIElementBase.HoverState.Enter)
                            {
                                uiElement.CurrentHoverState = UIElementBase.HoverState.Stay;
                            }
                            else if (uiElement.CurrentHoverState == UIElementBase.HoverState.Leave || uiElement.CurrentHoverState == UIElementBase.HoverState.None)
                            {
                                uiElement.CurrentHoverState = UIElementBase.HoverState.Enter;
                            }

                            /*if (uiElement.Hovered)
                            {
                                uiElement.HoverStay();
                            }
                            else
                            {
                                uiElement.HoverEnter();
                            }*/
                        }
                        else //if (uiElement.Hovered)
                        {
                            if (uiElement.CurrentHoverState == UIElementBase.HoverState.Stay || uiElement.CurrentHoverState == UIElementBase.HoverState.Enter)
                            {
                                uiElement.CurrentHoverState = UIElementBase.HoverState.Leave;
                            }
                            else if (uiElement.CurrentHoverState == UIElementBase.HoverState.Leave)
                            {
                                uiElement.CurrentHoverState = UIElementBase.HoverState.None;
                            }

                            //uiElement.HoverLeave();
                        }

                        //uiElement.Hovered = hoverFrame;

                        if (uiElement.CurrentHoverState != previousHoverState)
                        {
                            uiElement.HoverUpdate();
                        }

                        uiElement.Update();
                        break;

                    default:
                        item.Update();
                        break;
                }

                item.Render();
            }
        }

        /*
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
        }*/

        

    }
}
