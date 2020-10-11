using RPGEngine2;
using System.Data;

namespace RPG.UI
{
    public class ControllerConnectedMessage : UIElementBase
    {
        // Creddit Hayley Jane Wakenshaw, https://www.asciiart.eu/computers/game-consoles
        private readonly char[] GAMEBOY = new char[] { 
            ' ', '_', '_', '_', ' ', 
            '|', '[', '_', ']', '|', 
            '|', '+', ' ', ';', '|', 
            '`', '-', '-', '-', '\'' };
        private const int GAMEBOY_WIDTH = 5;
        private const int GAMEBOY_HEIGHT = 4;
        private readonly Vector2 GAMEBOY_OFFSET = Vector2.One;
        public string Message;

        public ControllerConnectedMessage()
        {
            Size = new Vector2(20, 6);
        }

        public override char[] Render()
        {
            char[] render = new char[Size.Product()];
            for (int y = 0; y < Size.RoundY; y++)
            {
                for (int x = 0; x < Size.RoundX; x++)
                {

                }
            }


            UIElementBase.DrawBorder(render, Size.RoundX, '#');
            return render;
        }
    }
}
