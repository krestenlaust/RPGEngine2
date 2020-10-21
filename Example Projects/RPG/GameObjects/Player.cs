using System;
using CommonComponents.UI;
using RPGEngine2;
using static RPGEngine2.EngineMain;

namespace RPG.GameObjects
{
    public class Player : GameCharacter
    {
        private const float VIBRATION_TIME = 0.5f;

        public Vector2 LookingDirection;
        private float previousHP;
        private float vibrationTimer;
        private bool vibrating;

        public Player(Progressbar healthbar, Vector2 position)
        {
            Position = position;
            Size = new Vector2(3, 3);
            Healthbar = healthbar;
            ShowHealthbar = true;

            HP = 50;
            MaxHP = 50;
            ZIndex = 100;

            PhysicsEnabled = true;
        }

        public override void Update()
        {
            vibrationTimer -= DeltaTime;

            if (previousHP != HP)
            {
                GameCode.Controller.SetVibration(0.5f, 0, GameCode.PlayerControllerID);
                vibrationTimer = VIBRATION_TIME;
                vibrating = true;
            }
            else if (vibrationTimer <= 0 && vibrating)
            {
                GameCode.Controller.StopVibration(GameCode.PlayerControllerID);
                vibrating = false;
            }
            previousHP = HP;

            UpdateHealthbar();
        }

        public override void Collision(GameObjectBase gameObject)
        {
            if (gameObject is Enemy)
            {
                HP -= 20 * DeltaTime;
            }
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
