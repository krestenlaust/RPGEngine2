namespace RPGEngine2
{
    public abstract class BaseObject
    {
        public Vector2 Size;
        public char[] RecentRendered;
        public bool Active = true;
        public byte ZIndex;
        internal Vector2 InternalPosition;
        internal bool isDestroyed;
        public abstract Vector2 Position { get; set; }

        /// <summary>
        /// Only called when <c>Active</c>.
        /// </summary>
        public virtual void Render()
        {
        }

        /// <summary>
        /// Called before <c>Render</c>. Only called when <c>Active</c>.
        /// </summary>
        public virtual void Update()
        {
        }

        public virtual void Destroy()
        {
            isDestroyed = true;
            Active = false;
        }

        public bool InsideBounds(Vector2 position)
        {
            return position.x >= InternalPosition.x && position.y >= InternalPosition.y && position.x < InternalPosition.x + Size.x && position.y < InternalPosition.y + Size.y;
        }
    }
}
