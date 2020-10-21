using RPGEngine2;

namespace BasicRPG.UI
{
    public class BasicTextbox : UIElementBase
    {
        public string Text;

        public BasicTextbox(Vector2 position, Vector2 size, string boxText)
        {
            this.Position = position;
            this.Size = size;
            this.Text = boxText;

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

        public override void Update()
        {
        }
    }
}
