using RPGEngine2.InputSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace RPGEngine2
{
    public static class EngineMain
    {
        private const int FPS_CAP = 120;
        private const int PHYSICS_FPS = 30;
        private const int TIME_PER_FRAME = 1000 / FPS_CAP;
        private const int TIME_PER_PHYSICS_FRAME = 1000 / PHYSICS_FPS;
        
        public static float DeltaTime { get; private set; } = 1;
        public static float FixedDeltaTime { get; private set; } = 1;
        private static readonly List<BaseObject> baseObjects = new List<BaseObject>();
        private static readonly List<BaseObject> instantiatedBaseObjects = new List<BaseObject>();
        private static bool isRunning;

        public static event Action OnStart;
        public static event Action OnUpdate;
        public static event Action OnFixedUpdate;

        public static void Instantiate(BaseObject baseObject) => instantiatedBaseObjects.Add(baseObject);
        public static void EngineStop() => isRunning = false;

        public static void EngineStart()
        {
            Stopwatch swUpdate = new Stopwatch();
            Stopwatch swFixedUpdate = new Stopwatch();
            swUpdate.Start();
            swFixedUpdate.Start();


            Renderer.ScreenHeight = (short)(Console.WindowHeight - 1);
            Renderer.ScreenWidth = (short)(Console.WindowWidth);

            Console.CursorVisible = false;

            isRunning = true;
            OnStart?.Invoke();

            while (isRunning)
            {
                // Cycles & Time
                DeltaTime = (float)swUpdate.Elapsed.TotalSeconds;
                Console.Title = "FPS: " + Math.Floor(1 / DeltaTime);
                swUpdate.Restart();
                
                // Input
                InputDeviceHandler.RefreshDevices();

                // Logic
                OnUpdate?.Invoke();

                bool physicsFrame = false;
                if (swFixedUpdate.ElapsedMilliseconds > TIME_PER_PHYSICS_FRAME)
                {
                    FixedDeltaTime = (float)swFixedUpdate.Elapsed.TotalSeconds;
                    swFixedUpdate.Restart();
                    
                    physicsFrame = true;
                    OnFixedUpdate?.Invoke();
                }

                UpdateBaseObjects(baseObjects, physicsFrame);

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

                while (swUpdate.ElapsedMilliseconds < TIME_PER_FRAME)
                    Thread.Sleep(1);
            }
        }

        private static void UpdateBaseObjects(List<BaseObject> baseObjects, bool physicsFrame)
        {
            List<GameObjectBase> physicsObjects;
            if (physicsFrame)
            {
                physicsObjects = (from obj in baseObjects
                                  where obj is GameObjectBase gameObject && gameObject.PhysicsEnabled
                                  select obj).Cast<GameObjectBase>().ToList();
            }
            else
            {
                physicsObjects = new List<GameObjectBase>();
            }

            foreach (BaseObject item in baseObjects)
            {
                if (item is null || !item.Active)
                    continue;

                switch (item)
                {
                    case GameObjectBase gameObject:
                        gameObject.Update();

                        if (physicsFrame)
                        {
                            gameObject.InternalPosition += gameObject.Velocity * FixedDeltaTime;

                            var collidingObjects = (from obj in physicsObjects
                                                   where obj != gameObject && Vector2.RectCollide(item.InternalPosition, item.Size, obj.Position, obj.Size)
                                                   select obj).ToList();

                            if (collidingObjects.Count > 0)
                            {
                                gameObject.Collision(collidingObjects);
                            }
                        }
                        break;

                    case UIElementBase uiElement:
                        if (InputDeviceHandler.InternalMouseDevice is null)
                        {
                            uiElement.Update();
                            break;
                        }

                        bool hoverFrame = uiElement.InsideBounds(new Vector2(InputDeviceHandler.InternalMouseDevice.x, InputDeviceHandler.InternalMouseDevice.y));
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
                        }
                        else
                        {
                            if (uiElement.CurrentHoverState == UIElementBase.HoverState.Stay || uiElement.CurrentHoverState == UIElementBase.HoverState.Enter)
                            {
                                uiElement.CurrentHoverState = UIElementBase.HoverState.Leave;
                            }
                            else if (uiElement.CurrentHoverState == UIElementBase.HoverState.Leave)
                            {
                                uiElement.CurrentHoverState = UIElementBase.HoverState.None;
                            }
                        }

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
    }
}
