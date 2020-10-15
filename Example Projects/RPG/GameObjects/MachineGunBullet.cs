using RPGEngine2;
using System.Collections.Generic;
using static RPGEngine2.EngineMain;

namespace RPG.GameObjects
{
    public class MachineGunBullet : GameObjectBase
    {
        private readonly float Damage = 3;
        private readonly float Speed = 2.5f;
        private readonly float AliveDuration = 1;
        private float AliveTimer;
        private readonly char[] Appearence = new char[] { '\0', '*', '\0' };

        public MachineGunBullet(Vector2 startPosition, Vector2 velocity)
        {
            Position = startPosition;
            Velocity = velocity * Speed;
            Size = new Vector2(3, 1);

            PhysicsEnabled = true;
        }

        public override void Collision(GameObjectBase gameObject)
        {
            if (gameObject is Enemy enemy)
            {
                enemy.HP -= Damage;
                enemy.Position += Velocity.Normalize();
                Destroy();
                return;
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
