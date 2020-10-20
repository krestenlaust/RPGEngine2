using System;
using CommonComponents.UI;
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
            ShowHealthbar = true;

            HP = 50;
            MaxHP = 50;
            ZIndex = 100;
        }

        public override void Update()
        {
            UpdateHealthbar();
        }

        public override char[] Render()
        {
            char[] render = new char[9];

            Vector2 lookpos = LookingDirection + Vector2.One;
            render[Size.RoundX * lookpos.RoundY + lookpos.RoundX] = '+';

            render[4] = '¤';

            return render;
        }
    }
}
