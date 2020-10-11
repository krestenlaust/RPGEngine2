using RPGEngine2;

namespace RPG
{
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

        public override void Render()
        {
            RecentRendered = new char[Size.RoundX];
            RecentRendered[0] = LEFT_BORDER;
            RecentRendered[RecentRendered.Length - 1] = RIGHT_BORDER;

            int barWidth = RecentRendered.Length - 2;

            for (int i = 1; i < RecentRendered.Length - 1; i++)
            {
                if (Progress * barWidth >= i - 1)
                {
                    RecentRendered[i] = Filled;
                }
                else
                {
                    RecentRendered[i] = Empty;
                }
            }
        }
    }
}
