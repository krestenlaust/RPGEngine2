using RPGEngine2;

namespace CommonComponents.UI
{
    // Originally created for 'RPG' project.
    public class Progressbar : UIElementBase
    {
        public const char LEFT_BORDER = '[';
        public const char RIGHT_BORDER = ']';
        public float Progress;
        public char Filled;
        public char Empty;

        public Progressbar(int fullWidth, char filled, char empty)
        {
            Size = new Vector2(fullWidth, 1);
            Filled = filled;
            Empty = empty;
            PositionOffset = new Vector2(-fullWidth / 2, 0);

            ZIndex = byte.MaxValue;
        }

        public override void Update()
        {

        }

        public override char[] Render()
        {
            char[] render = new char[Size.RoundX];
            render[0] = LEFT_BORDER;
            render[render.Length - 1] = RIGHT_BORDER;

            int barWidth = render.Length - 2;

            for (int i = 1; i < render.Length - 1; i++)
            {
                if (Progress * barWidth >= i - 1)
                {
                    render[i] = Filled;
                }
                else
                {
                    render[i] = Empty;
                }
            }

            return render;
        }
    }
}
