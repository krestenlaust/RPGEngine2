using RPGEngine2;
using static RPG.GameCode;

namespace RPG.GameObjects
{
    public class Player : GameCharacter
    {
        public Player(Progressbar healthbar, Vector2 position)
        {
            Position = position;
            Size = new Vector2(3, 1);
            Healthbar = healthbar;

            HP = 50;
            MaxHP = 50;
        }

        public override void Update()
        {
            UpdateHealthbar();
        }

        public override void Render()
        {
            RecentRendered = new char[3];
            RecentRendered[1] = '¤';

            if (Mouse.x > Position.x)
            {
                RecentRendered[2] = '—';
            }
            else if (Mouse.x < Position.x)
            {
                RecentRendered[0] = '—';
            }
        }
    }
}
