using RPGEngine2;
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
        }

        public override void Update()
        {
            AliveTimer += DeltaTime;

            foreach (var item in GameCode.Enemies)
            {
                if (Position.RoundX != item.Position.RoundX)
                    continue;

                if (Position.RoundY != item.Position.RoundY)
                    continue;

                item.HP -= Damage;
                item.Position += Velocity.Normalize();
                Destroy();
                return;
            }

            if (AliveTimer >= AliveDuration)
            {
                RecentRendered = new char[] { '*' };
                Destroy();
            }
        }
    }

}
