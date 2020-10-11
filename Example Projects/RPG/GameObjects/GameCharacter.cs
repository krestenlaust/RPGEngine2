using RPGEngine2;
using System;

namespace RPG.GameObjects
{
    public class GameCharacter : GameObjectBase
    {
        public Progressbar Healthbar;
        public float HP;
        public float MaxHP;
        protected bool ShowHealthbar;

        public void UpdateHealthbar()
        {
            Healthbar.Active = ShowHealthbar;

            if (Healthbar is null || !ShowHealthbar)
                return;

            Healthbar.Position = Position + new Vector2(0, -1);
            Healthbar.Progress = Math.Min((float)HP / MaxHP, 1);
        }

        public override void Destroy()
        {
            Healthbar.Destroy();
            base.Destroy();
        }

        public override char[] Render()
        {
            return new char[] { };
        }
    }
}
