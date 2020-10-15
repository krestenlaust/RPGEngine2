using RPGEngine2;

namespace BasicRPG.UI
{
    public class BasicTextbox : UIElementBase
    {
        public string Text;

        public BasicTextbox(Vector2 Position, Vector2 Size, string BoxText)
        {
            this.Position = Position;
            this.Size = Size;
            this.Text = BoxText;

            ZIndex = byte.MaxValue;
        }

        public override char[] Render()
        {
            char[] render = new char[Size.Product()];

            for (int i = 0; i < render.Length; i++)
            {
                if (Text.Length > i)
                {
                    render[i] = Text[i];

                }
                else
                {
                    render[i] = ' ';
                }
            }

            return render;
        }
    }
}
