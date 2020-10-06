﻿using RPGEngine2;
using static RPGEngine2.EngineMain;
using static RPG.GameCode;

namespace RPG
{
    public class AnimatedTextBox : UIElementBase
    {
        public const float ANIMATE_SPEED = 0.4f;
        private const int DRAG_BUTTON_INDEX = 1;
        public string BoxText;
        private char BorderChar = '#';
        private int IndentedLetterIndex = 0;
        private float Timer = 0;
        private bool BeingDragged = false;

        public AnimatedTextBox(Vector2 Position, Vector2 Size, string BoxText)
        {
            this.Position = Position;
            this.Size = Size;
            this.BoxText = BoxText;
            this.RecentRendered = new char[Size.Product()];

            ZIndex = byte.MaxValue;
        }

        public override void HoverUpdate()
        {
            switch (CurrentHoverState)
            {
                case HoverState.Enter:
                    BorderChar = '$';
                    break;
                case HoverState.Leave:
                    BorderChar = '#';
                    break;
                default:
                    break;
            }
        }

        public override void Update()
        {
            if (BeingDragged && Mouse.ButtonReleased(DRAG_BUTTON_INDEX))
            {
                BeingDragged = false;
            }

            if (BeingDragged)
            {
                Position = new Vector2(Mouse.x, Mouse.y) - Size / 2;
                BorderChar = '@';
            }

            if (CurrentHoverState == HoverState.Stay)
            {
                Timer += DeltaTime;
                if (Timer >= ANIMATE_SPEED)
                {
                    IndentedLetterIndex = (IndentedLetterIndex + 1) % BoxText.Length;

                    Timer -= ANIMATE_SPEED;
                }

                if (Mouse.ButtonDown(0))
                {
                    BorderChar = '@';
                }
                else if (Mouse.ButtonReleased(0))
                {
                    BorderChar = '$';
                }

                if (Mouse.ButtonDown(DRAG_BUTTON_INDEX))
                {
                    BeingDragged = true;
                }
            }
        }

        public override void Render()
        {
            RecentRendered = new char[RecentRendered.Length];

            for (int y = 0; y < Size.y; y++)
                for (int x = 0; x < Size.x; x++)
                {
                    int index = (int)Size.x * y + x;
                    if (y == 0 || y == Size.y - 1 || x == 0 || x == Size.x - 1)
                    {
                        RecentRendered[index] = BorderChar;
                        continue;
                    }

                    int letterIndex = x - 2;

                    if (x <= 1 || y <= 0 || y > 2)
                    {
                        RecentRendered[index] = ' ';
                        continue;
                    }

                    if (y == 1)
                    {
                        if (!Hovered || letterIndex != IndentedLetterIndex)
                        {
                            RecentRendered[index] = ' ';
                            continue;
                        }
                    }

                    if (y == 2 && letterIndex == IndentedLetterIndex && Hovered)
                    {
                        RecentRendered[index] = ' ';
                        continue;
                    }

                    if (letterIndex >= BoxText.Length)
                    {
                        RecentRendered[index] = ' ';
                        continue;
                    }

                    RecentRendered[index] = BoxText[letterIndex];
                }
        }
    }

    

    
}
