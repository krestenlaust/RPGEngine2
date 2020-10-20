using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGEngine2
{
    public static class Renderer
    {
        public static short ScreenHeight { get; internal set; }
        public static short ScreenWidth { get; internal set; }
        /// <summary>
        /// The position of the top-left corner of screen relative to the world (the console buffer) coordinates.
        /// </summary>
        public static Vector2 ScreenPosition { get; private set; } = Vector2.Zero;
        public static bool isDebugStringsEnabled { get; set; }
        /// <summary>
        /// The character that is drawn when no other gameobject is. Causes slight overhead when not null as 
        /// <c>ResetScreenBuffer</c> will fill buffer with this character on call.
        /// </summary>
        public static char VoidfillChar { get; set; }
        internal static char[] ScreenBuffer;
        internal static List<DebugString> DebugStrings = new List<DebugString>();

        public static void SetScreenPosition(Vector2 newPosition)
        {
            Console.SetWindowPosition(newPosition.RoundX, newPosition.RoundY);
            ScreenPosition = newPosition;
        }

        public static void ResetScreenBuffer()
        {
            ScreenBuffer = new char[ScreenWidth * ScreenHeight];
            
            if (VoidfillChar == '\0')
                return;

            for (int i = 0; i < ScreenBuffer.Length; i++)
            {
                ScreenBuffer[i] = VoidfillChar;
            }
        }

        public static void PerformRendering(List<BaseObject> baseObjects)
        {
            FillScreenBuffer(ScreenBuffer, baseObjects, ScreenWidth, ScreenHeight);
        }

        public static void FlushScreenBuffer()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(ScreenBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="sizeX"></param>
        /// <param name="destination"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <param name="ignoreNullSource">Makes <c>\0</c> characters see-through</param>
        private static void CompositeMatrix(char[] source, int positionX, int positionY, int sizeX, char[] destination, int screenWidth, int screenHeight)
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

                if (EngineMain.GameConfig.isNullSeeThrough && source[i] == '\0')
                    continue;

                destination[screenIndex] = source[i];
            }
        }

        private static bool isOffScreen(BaseObject obj)
        {
            return obj.InternalPosition.x + obj.Size.x < ScreenPosition.x || obj.InternalPosition.x >= ScreenWidth + ScreenPosition.x ||
                obj.InternalPosition.y + obj.Size.y < ScreenPosition.y || obj.InternalPosition.y >= ScreenHeight + ScreenPosition.y;
        }

        /// <summary>
        /// Draws objects to the char array based on their position and size.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="baseObjects"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        private static void FillScreenBuffer(char[] buffer, List<BaseObject> baseObjects, int screenWidth, int screenHeight)
        {
            var sortedObjects = from obj in baseObjects
                                where obj?.Active == true && !isOffScreen(obj)
                                orderby obj.ZIndex ascending
                                select obj;

            foreach (BaseObject obj in sortedObjects)
            {
                CompositeMatrix(
                    obj.Render(),
                    obj.InternalPosition.RoundX,
                    obj.InternalPosition.RoundY,
                    obj.Size.RoundX,
                    buffer,
                    screenWidth,
                    screenHeight
                    );
            }

            if (isDebugStringsEnabled)
            {
                for (int i = 0; i < DebugStrings.Count; i++)
                {
                    CompositeMatrix(
                        DebugStrings[i].message,
                        DebugStrings[i].Position.RoundX,
                        DebugStrings[i].Position.RoundY,
                        DebugStrings[i].message.Length,
                        buffer,
                        screenWidth,
                        screenHeight
                        );
                }
            }
        }
    }
}
