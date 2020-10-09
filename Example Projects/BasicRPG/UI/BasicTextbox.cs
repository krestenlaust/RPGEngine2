using RPGEngine2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRPG.UI
{
    public class BasicTextbox : UIElementBase
    {
        public string Text;

        public BasicTextbox(Vector2 Position, Vector2 Size, string BoxText)
        {
            this.Position = Position;
            this.Size = Size;
            this.RecentRendered = new char[Size.Product()];
            this.Text = BoxText;

            ZIndex = byte.MaxValue;
        }

        public override void Render()
        {
            for (int i = 0; i < RecentRendered.Length; i++)
            {
                if (Text.Length > i)
                {
                    RecentRendered[i] = Text[i];

                }
                else
                {
                    RecentRendered[i] = ' ';
                }
            }
        }
    }
}
