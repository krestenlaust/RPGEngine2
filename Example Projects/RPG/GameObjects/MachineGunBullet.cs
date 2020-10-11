using RPGEngine2;
using System.Collections.Generic;
using static RPGEngine2.EngineMain;

namespace RPG.GameObjects
{
    public class MachineGunBullet : GameObjectBase
    {
        private readonly float Damage = 4;
        private readonly float Speed = 1.5f;
        private readonly float AliveDuration = 1.5f;
        private float AliveTimer;
        private readonly char[] Appearence = new char[] { '\0', '*', '\0' };

        public MachineGunBullet(Vector2 startPosition, Vector2 velocity)
        {
            Position = startPosition;
            Velocity = velocity * Speed;
            Size = new Vector2(3, 1);

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
                Destroy();
            }
        }

        public override char[] Render() => Appearence;
    }

}
