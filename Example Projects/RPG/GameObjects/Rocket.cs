using RPGEngine2;
using static RPGEngine2.EngineMain;

namespace RPG
{
    public class Rocket : GameObjectBase
    {
        private float AliveTimer;
        private float AliveDuration;

        public Rocket(Vector2 startPosition, Vector2 velocity, float aliveDuration)
        {
            Position = startPosition;
            Velocity = velocity;
            AliveDuration = aliveDuration;
            Size = new Vector2(1, 1);

            RecentRendered = new char[] { '!' };
        }

        public override void Update()
        {
            AliveTimer += DeltaTime;

            if (AliveTimer >= AliveDuration)
            {
                Instantiate(new RPGExplosion(Position));
                Destroy();
            }
        }
    }
}
