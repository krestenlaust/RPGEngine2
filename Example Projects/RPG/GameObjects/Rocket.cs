using RPG.GameObjects;
using RPGEngine2;
using System.Collections.Generic;
using System.Linq;
using static RPGEngine2.EngineMain;

namespace RPG
{
    public class Rocket : GameObjectBase
    {
        public static readonly float ControllerRumble = 0.5f;
        private readonly float AliveDuration;
        private float aliveTimer;
        private float AnimationTimer = 1;
        private bool OnTheSides = false;
        private char horizontalThing;
        private char verticalThing;

        public Rocket(Vector2 startPosition, Vector2 velocity, float aliveDuration)
        {
            PositionOffset = new Vector2(-1, -1);
            Position = startPosition;
            Velocity = velocity;
            this.AliveDuration = aliveDuration;
            Size = new Vector2(3, 3);
            ZIndex = 101;

            PhysicsEnabled = true;
        }

        public override void Collision(GameObjectBase gameObject)
        {
            if (gameObject is Enemy)
            {
                aliveTimer = AliveDuration;
            }
        }

        public override void Update()
        {
            aliveTimer += DeltaTime;

            if (aliveTimer >= AliveDuration)
            {
                GameCode.Controller.SetVibration(ControllerRumble, 0, GameCode.PlayerControllerID);
                Instantiate(new RPGExplosion(Position));
                Destroy();
            }


            AnimationTimer += DeltaTime;

            float compareTime = (AliveDuration - aliveTimer) / AliveDuration;
            if (AnimationTimer >= compareTime)
            {
                OnTheSides = !OnTheSides;
                AnimationTimer -= compareTime;

                if (OnTheSides)
                {
                    verticalThing = '\0';
                    horizontalThing = '-';
                }
                else
                {
                    verticalThing = 'l';
                    horizontalThing = '\0';
                }
            }
        }

        public override char[] Render()
        {
            return new char[]
                {
                    '\0',           verticalThing, '\0',
                    horizontalThing, '!',          horizontalThing,
                    '\0',           verticalThing, '\0'
                };

        }
    }
}
