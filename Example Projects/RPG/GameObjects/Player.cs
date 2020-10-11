using RPGEngine2;

namespace RPG.GameObjects
{
    public class Player : GameCharacter
    {
        public Vector2 LookingDirection;

        public Player(Progressbar healthbar, Vector2 position)
        {
            Position = position;
            Size = new Vector2(3, 3);
            Healthbar = healthbar;

            HP = 50;
            MaxHP = 50;
            ZIndex = 100;
        }

        public override void Update()
        {
            UpdateHealthbar();
        }

        public override void Render()
        {
            RecentRendered = new char[9];

            Vector2 lookpos = LookingDirection + Vector2.One;
            RecentRendered[Size.RoundX * lookpos.RoundY + lookpos.RoundX] = '*';

            RecentRendered[4] = '¤';
        }
    }
}
