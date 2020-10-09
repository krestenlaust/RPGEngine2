using System;

namespace RPGEngine2
{
    public struct Mathf
    {
        /// <summary>
        /// Interpolate liniarily between a and b by t.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Lerp(float a, float b, float t)
        {
            return a * (1 - t) + b * t;
        }
    }

    /// <summary>
    /// Vector2 holds 2-dimensionel coordinate set(x and y).
    /// </summary>
    public struct Vector2 : IEquatable<Vector2>
    {
        public float x { get; private set; }
        public float y { get; private set; }
        public int RoundX
        {
            get
            {
                return (int)Math.Round(x);
            }
        }

        public int RoundY
        {
            get
            {
                return (int)Math.Round(y);
            }
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Shorthand for writing Vector2(0, 0).
        /// </summary>
        public static Vector2 Zero { get; } = new Vector2(0, 0);
        /// <summary>
        /// Shorthand for writing Vector2(0, 0).
        /// </summary>
        public static Vector2 One { get; } = new Vector2(1, 1);
        /// <summary>
        /// Shorthand for writing Vector(1, 0).
        /// </summary>
        public static Vector2 Right { get; } = new Vector2(1, 0);
        /// <summary>
        /// Shorthand for writing Vector(-1, 0).
        /// </summary>
        public static Vector2 Left { get; } = new Vector2(-1, 0);
        /// <summary>
        /// Shorthand for writing Vector(0, -1).
        /// </summary>
        public static Vector2 Up { get; } = new Vector2(0, -1);
        /// <summary>
        /// Shorthand for writing Vector(0, 1).
        /// </summary>
        public static Vector2 Down { get; } = new Vector2(0, 1);

        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt((x * x) + (y * y));
            }
        }

        /// <summary>
        /// Calculates the position that is a percentage(<code>t</code>) of the travel between position a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t));
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt(Math.Pow((b.x - a.x), 2) + Math.Pow((b.y - a.y), 2));
        }

        private static readonly int[] Directional = new int[] { 1, 1, 0, -1, -1, -1, 0, 1 };
        private const int DIRECTIONAL_LEN = 8;

        public int OneDimensional(int width)
        {
            return width * (int)y + (int)x;
        }

        public int Product()
        {
            return (int)x * (int)y;
        }

        public static bool RectCollide(Vector2 positionA, Vector2 sizeA, Vector2 positionB, Vector2 sizeB)
        {
            return positionA.x < positionB.x + sizeB.x && positionA.x + sizeA.x > positionB.x &&
                positionA.y < positionB.y + sizeB.y && positionA.y + sizeA.y > positionB.y;
        }

        /*
        public static bool RectCollide(Vector2 position1, Vector2 size1, Vector2 position2, Vector2 size2)
        {
            float rect1Left = position1.x;
            float rect1Right = position1.x + size1.x;

            float rect1Bottom

            foreach (var item in EngineMain.GameObjects)
            {

                if (RectA.Left < RectB.Right && RectA.Right > RectB.Left && RectA.Top > RectB.Bottom && RectA.Bottom < RectB.Top)
                {

                }
            }
        }*/

        public Vector2 Normalize()
        {
            return this / Magnitude;
        }

        /// <summary>
        /// Get cardinal and 4 other neighbouring coordinates.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector2[] Neighbours()
        {
            Vector2[] neighbours = new Vector2[DIRECTIONAL_LEN];

            for (int i = 0; i < DIRECTIONAL_LEN; i++)
            {
                int x = Directional[(i + 2) % DIRECTIONAL_LEN] + (int)this.x;
                int y = Directional[i] + (int)this.y;

                neighbours[i] = new Vector2(x, y);
            }

            return neighbours;
        }

        /// <summary>
        /// Compares this with another Vector2.
        /// </summary>
        public bool Equals(Vector2 vector2)
        {
            return x == vector2.x && y == vector2.y;
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.x != b.x || a.y != b.y;
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }
        public static Vector2 operator +(Vector2 a, float b)
        {
            return new Vector2(a.x + b, a.y + b);
        }
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }
        public static Vector2 operator *(float d, Vector2 a)
        {
            return new Vector2(a.x * d, a.y * d);
        }
        public static Vector2 operator *(Vector2 a, float d)
        {
            return new Vector2(a.x * d, a.y * d);
        }
        public static Vector2 operator *(int d, Vector2 a)
        {
            return new Vector2(a.x * d, a.y * d);
        }
        public static Vector2 operator *(Vector2 a, int d)
        {
            return new Vector2(a.x * d, a.y * d);
        }
        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
        public static Vector2 operator *(decimal d, Vector2 a)
        {
            return new Vector2(a.x * (float)d, a.y * (float)d);
        }
        public static Vector2 operator *(Vector2 a, decimal d)
        {
            return new Vector2(a.x * (float)d, a.y * (float)d);
        }
        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2(a.x / b, a.y / b);
        }
        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }
    }
}
