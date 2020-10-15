using RPG.GameObjects.Particles;
using RPGEngine2;
using static RPGEngine2.EngineMain;

namespace RPG.GameObjects
{
    public class Enemy : GameCharacter
    {
        private const int DEFAULT_HP = 15;
        private readonly char[] Appearance = new char[] { '#' };

        public Enemy(Progressbar healthbar, Vector2 position)
        {
            HP = DEFAULT_HP;
            MaxHP = DEFAULT_HP;
            Position = position;
            Healthbar = healthbar;
            Size = new Vector2(1, 1);
            ShowHealthbar = true;

            PhysicsEnabled = true;
        }

        public override char[] Render() => Appearance;

        public override void Update()
        {
            UpdateHealthbar();

            if (HP <= 0)
            {
                Instantiate(new DeathExplosion(Position));
                Destroy();
            }
        }
    }
}
