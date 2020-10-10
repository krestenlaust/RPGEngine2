using RPGEngine2;
using System.Collections.Generic;
using static RPGEngine2.EngineMain;

namespace RPG.GameObjects
{
    public class MachineGunBullet : GameObjectBase
    {
        private int Damage = 5;
        private readonly float AliveDuration = 2;
        private float AliveTimer;

        public MachineGunBullet(Vector2 startPosition, Vector2 velocity)
        {
            Position = startPosition;
            Velocity = velocity;
            Size = new Vector2(1, 1);
            RecentRendered = new char[] { '\'' };

            PhysicsEnabled = true;
        }

        public override void Collision(List<GameObjectBase> gameObjects)
        {
            foreach (var item in gameObjects)
            {
                if (item is Enemy enemy)
                {
                    enemy.HP -= Damage;
                    enemy.Position += Velocity.Normalize();
                    Destroy();
                    return;
                }
            }
        }

        public override void Update()
        {
            AliveTimer += DeltaTime;

            if (AliveTimer >= AliveDuration)
            {
                RecentRendered = new char[] { '*' };
                Destroy();
            }
        }
    }

}
