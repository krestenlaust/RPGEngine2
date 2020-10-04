using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGEngine2
{
    public static class Renderer
    {
        public static short ScreenHeight { get; internal set; } 
        public static short ScreenWidth { get; internal set; }
        internal static char[] ScreenBufferGame;
        internal static char[] ScreenBufferUI;

        public static void ResetScreenBuffers()
        {
            ScreenBufferUI = new char[ScreenWidth * ScreenHeight];
            ScreenBufferGame = new char[ScreenWidth * ScreenHeight];
        }

        public static void PerformRendering(List<UIElementBase> uiElements, List<GameObjectBase> gameObjects)
        {
            CompositeScreenBuffer(uiElements, gameObjects);
        }

        public static void WriteStandardOut()
        {
            char[] outputArray = new char[ScreenBufferGame.Length];
            for (int i = 0; i < ScreenBufferGame.Length; i++)
            {
                if (ScreenBufferUI[i] == '\0')
                    outputArray[i] = ScreenBufferGame[i];
                else
                    outputArray[i] = ScreenBufferUI[i];
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(outputArray);
        }

        private static void CompositeScreenBuffer(List<UIElementBase> uiElements, List<GameObjectBase> gameObjects)
        {
            FillScreenBuffer(ScreenBufferUI, uiElements, ScreenWidth, ScreenHeight);
            FillScreenBuffer(ScreenBufferGame, gameObjects, ScreenWidth, ScreenHeight);
        }

        private static void Composite2dArray(char[] source, int positionX, int positionY, int sizeX, char[] destination, int screenWidth, int screenHeight, bool replaceNull=true)
        {
            int cellX = positionX - 1;
            int cellY = positionY;

            for (int i = 0; i < source.Length; i++)
            {
                if (i % sizeX == 0 && i != 0)
                {
                    cellX = positionX - 1;
                    cellY++;
                }

                cellX++;

                if (cellX < 0 || cellX >= screenWidth || cellY < 0 || cellY >= screenHeight)
                    continue;

                int screenIndex = screenWidth * cellY + cellX;
                if (screenIndex >= destination.Length || screenIndex < 0)
                    continue;

                if (!replaceNull && source[i] == '\0')
                    continue;

                destination[screenIndex] = source[i];
            }
        }

        private static void FillScreenBuffer(char[] buffer, List<GameObjectBase> gameObjects, int screenWidth, int screenHeight)
        {
            var sortedGameObjects = from obj in gameObjects
                                    orderby obj.ZIndex descending
                                    select obj;

            foreach (GameObjectBase item in sortedGameObjects)
            {
                if (!item.Active)
                    continue;

                Composite2dArray(item.RecentRendered, item.InternalPosition.RoundX, item.InternalPosition.RoundY, item.Size.RoundX, buffer, screenWidth, screenHeight);
            }
        }

        private static void FillScreenBuffer(char[] buffer, List<UIElementBase> elements, int screenWidth, int screenHeight)
        {
            var sortedUIElements = from obj in elements
                                   orderby obj.ZIndex descending
                                   select obj;

            foreach (UIElementBase item in sortedUIElements)
            {
                if (!item.Active)
                    continue;

                Composite2dArray(item.RecentRendered, item.InternalPosition.RoundX, item.InternalPosition.RoundY, item.Size.RoundX, buffer, screenWidth, screenHeight, false);
            }
        }
    }
}
