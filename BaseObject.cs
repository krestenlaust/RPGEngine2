namespace RPGEngine2
{
    public abstract class BaseObject
    {
        /// <summary>
        /// Size of the rectangle that represents the object, <c>Size.RoundX</c> and <c>Size.RoundY</c> are used.
        /// </summary>
        public Vector2 Size;
        public char[] RecentRendered;
        /// <summary>
        /// <c>Update()</c> and <c>Render()</c> is only called when <c>Active</c> is true.
        /// </summary>
        public bool Active = true;
        /// <summary>
        /// Specifies when an object is to be drawn. Objects with lower <c>ZIndex</c> are drawn first, resulting in them appearing below objects with higher <c>ZIndex</c>.
        /// </summary>
        public byte ZIndex;
        /// <summary>
        /// The position of the top-left corner.
        /// </summary>
        internal Vector2 InternalPosition;
        internal bool isDestroyed;
        /// <summary>
        /// Can be implemented to for example, represent the middle of the object instead of the top-left corner.
        /// </summary>
        public abstract Vector2 Position { get; set; }

        /// <summary>
        /// Only called when <c>Active</c> is true.
        /// </summary>
        public virtual void Render()
        {
        }

        /// <summary>
        /// Called before <c>Render</c>. Only called when <c>Active</c> is true.
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// Marks object for destruction. Is removed before next gameloop takes place.
        /// </summary>
        public virtual void Destroy()
        {
            isDestroyed = true;
            Active = false;
        }

        /// <summary>
        /// Returns whether <c>position</c> is located inside the rectangle defined by <c>Size</c> and <c>Position</c>.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool InsideBounds(Vector2 position)
        {
            return position.x >= InternalPosition.x && position.y >= InternalPosition.y && position.x < InternalPosition.x + Size.x && position.y < InternalPosition.y + Size.y;
        }
    }
}
