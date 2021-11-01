using RPGEngine2;
using System;
using static RPG.GameCode;

namespace RPG.UI
{
    public class MenuOption : UIElementBase
    {
        // Stored as constants to prevent inconsistensies in styling.
        private const char DEFAULT_CHAR = '\u2588'; //2593
        private const char HOVER_CHAR = '\u2592';
        private const int TEXT_LEFT_MARGIN = 2;

        public int BoxHeight = 5;
        public string ButtonText;
        public char BorderChar = DEFAULT_CHAR;
        public Action OnClick;

        public MenuOption(string buttonText, Vector2 position, int optionWidth, Action onClick = null)
        {
            ButtonText = buttonText;
            Position = position;
            Size = new Vector2(optionWidth, BoxHeight);
            OnClick = onClick;

            ZIndex = 110;
        }

        public override void HoverUpdate()
        {
            switch (CurrentHoverState)
            {
                case HoverState.Enter:
                    BorderChar = HOVER_CHAR;
                    break;
                case HoverState.Leave:
                    BorderChar = DEFAULT_CHAR;
                    break;
            }
        }

        public override void Update()
        {
            if (CurrentHoverState == HoverState.Stay)
            {
                if (Mouse.ButtonReleased(0))
                {
                    OnClick?.Invoke();
                }
            }
        }

        public override char[] Render()
        {
            char[] render = new char[Size.Product()];

            for (int y = 0; y < Size.y; y++)
            {
                for (int x = 0; x < Size.x; x++)
                {
                    int index = (int)Size.x * y + x;
                    render[index] = ' ';

                    if (y != 2)
                    {
                        break;
                    }

                    if (x < TEXT_LEFT_MARGIN)
                    {
                        continue;
                    }

                    if (x - TEXT_LEFT_MARGIN >= ButtonText.Length)
                    {
                        break;
                    }

                    render[index] = ButtonText[x - TEXT_LEFT_MARGIN];
                }
            }

            DrawBorder(render, (int)Size.x, BorderChar);

            return render;
        }
    }
}
