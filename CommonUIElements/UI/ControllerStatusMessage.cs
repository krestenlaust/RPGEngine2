using RPGEngine2;
using System;
using static RPGEngine2.EngineMain;

namespace CommonComponents.UI
{
    // Originally created for 'RPG' project.
    public class ControllerStatusMessage : UIElementBase
    {
        // Credit: Hayley Jane Wakenshaw, https://www.asciiart.eu/computers/game-consoles
        private readonly char[] GAMEBOY = new char[] {
            ' ', '_', '_', '_', ' ',
            '|', '[', '_', ']', '|',
            '|', '+', ' ', ';', '|',
            '`', '-', '-', '-', '\'' };
        private const int GAMEBOY_WIDTH = 5;
        private const int GAMEBOY_HEIGHT = 4;
        private readonly Vector2 GAMEBOY_POSITION = Vector2.One;
        private readonly Vector2 MESSAGE_POSITION = new Vector2(7, 3);
        private const float ANIMATION_DURATION = 2;
        private const float POPUP_DURATION = 1.5f;
        public string Message;
        public float DismissTimer;
        private float animationTimer = ANIMATION_DURATION;
        private Vector2 AnimationStartPosition;
        private Vector2 AnimationEndPosition;

        public ControllerStatusMessage()
        {
            Size = new Vector2(35, 6);
            Position = new Vector2(0, -Size.y);
            ZIndex = 150;
        }

        public void Popup(string message)
        {
            DismissTimer = POPUP_DURATION;
            Message = message;
            animationTimer = 0;
            AnimationStartPosition = Position;
            AnimationEndPosition = new Vector2(0, -1);
        }

        public void Dismiss()
        {
            animationTimer = 0;
            AnimationStartPosition = Position;
            AnimationEndPosition = new Vector2(0, -Size.y);
        }

        public override void Update()
        {
            // Animate
            if (animationTimer < ANIMATION_DURATION)
            {
                animationTimer += DeltaTime;

                Position = Vector2.Lerp(AnimationStartPosition, AnimationEndPosition, Math.Min(animationTimer / ANIMATION_DURATION, 1));
            }
            else
            {
                if (DismissTimer > 0)
                {
                    DismissTimer -= DeltaTime;
                }
                else
                {
                    Dismiss();
                }
            }
        }

        public override char[] Render()
        {
            char[] render = new char[Size.Product()];

            for (int i = 0; i < render.Length; i++)
            {
                render[i] = ' ';
            }

            for (int y = 0; y < GAMEBOY_HEIGHT; y++)
            {
                for (int x = 0; x < GAMEBOY_WIDTH; x++)
                {
                    Vector2 position = new Vector2(x, y) + GAMEBOY_POSITION;
                    render[position.OneDimensional(Size.RoundX)] = GAMEBOY[y * GAMEBOY_WIDTH + x];
                }
            }

            if (!(Message is null))
            {
                for (int i = 0; i < Message.Length; i++)
                {
                    Vector2 charPosition = new Vector2(i, 0) + MESSAGE_POSITION;
                    if (charPosition.x >= Size.RoundX)
                        break;

                    render[charPosition.OneDimensional(Size.RoundX)] = Message[i];
                }
            }

            UIElementBase.DrawBorder(render, Size.RoundX, '#');
            return render;
        }
    }
}
