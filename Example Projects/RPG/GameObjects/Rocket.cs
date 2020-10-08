using RPG.GameObjects;
using RPGEngine2;
using System;
using System.Collections.Generic;
using System.Linq;
using static RPGEngine2.EngineMain;

namespace RPG
{
    public class Rocket : GameObjectBase
    {
        private readonly float AliveDuration;
        private float aliveTimer;
        private float AnimationTimer = 1;
        private bool OnTheSides = false;

        public Rocket(Vector2 startPosition, Vector2 velocity, float aliveDuration)
        {
            PositionOffset = new Vector2(-1, -1);
            Position = startPosition;
            Velocity = velocity;
            this.AliveDuration = aliveDuration;
            Size = new Vector2(3, 3);

            PhysicsEnabled = true;
        }

        public override void Collision(List<GameObjectBase> gameObjects)
        {
            if (gameObjects.Any(obj => obj is Enemy))
            {
                aliveTimer = AliveDuration;
            }
        }

        public override void Render()
        {
            AnimationTimer += DeltaTime;

            if (AnimationTimer < (AliveDuration - aliveTimer) / AliveDuration)
                return;

            char verticalThing;
            char horizontalThing;


            AnimationTimer -= (AliveDuration - aliveTimer) / AliveDuration;
            OnTheSides = !OnTheSides;

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

            RecentRendered = new char[]
                {
                    '\0',           verticalThing, '\0',
                    horizontalThing, '!',          horizontalThing,
                    '\0',           verticalThing, '\0'
                };

        }

        public override void Update()
        {
            aliveTimer += DeltaTime;

            if (aliveTimer >= AliveDuration)
            {
                Instantiate(new RPGExplosion(Position));
                Destroy();
            }
        }
    }
}
