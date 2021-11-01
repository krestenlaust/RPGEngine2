using CommonComponents.UI;
using RPG.GameObjects.Particles;
using RPGEngine2;
using static RPGEngine2.EngineMain;

namespace RPG.GameObjects
{
    public class Enemy : GameCharacter
    {
        private const float MOVE_SPEED = 2f;
        private const int DEFAULT_HP = 20;
        private const float HP_TIMER_DURATION = 2.5f;

        private readonly char[] Appearance = new char[] { '#' };
        private bool backedAway = false;
        private float hideHPTimer = HP_TIMER_DURATION;
        private float previousHP;

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

        public override void Collision(GameObjectBase gameObject)
        {
            if (!backedAway && gameObject is Enemy enemy)
            {
                Position += (Position - enemy.Position).Normalize();
                backedAway = true;
            }
        }

        public override void Update()
        {
            if (MainMenu.MenuShown)
            {
                return;
            }

            backedAway = false;
            Position += (GameCode.PlayerObj.Position - Position).Normalize() * DeltaTime * MOVE_SPEED * Vector2.ScreenRatio;

            hideHPTimer -= DeltaTime;

            if (hideHPTimer <= 0)
            {
                ShowHealthbar = false;
            }

            if (previousHP != HP)
            {
                ShowHealthbar = true;
                hideHPTimer = HP_TIMER_DURATION;
            }
            previousHP = HP;

            UpdateHealthbar();

            if (HP <= 0)
            {
                Instantiate(new DeathExplosion(Position));
                Destroy();
                GameCode.EnemyCount--;
            }
        }
    }
}
