using RPGEngine2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.GameObjects
{
    public class GameCharacter : GameObjectBase
    {
        public Progressbar Healthbar;
        public int HP;
        public int MaxHP;

        public void UpdateHealthbar()
        {
            if (Healthbar is null)
            {
                return;
            }

            Healthbar.Position = Position + new Vector2(0, -1);
            Healthbar.Progress = Math.Min((float)HP / MaxHP, 1);
        }

        public override void Destroy()
        {
            Healthbar.Destroy();
            base.Destroy();
        }
    }
}
