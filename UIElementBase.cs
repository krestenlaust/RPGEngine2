namespace RPGEngine2
{
    public abstract class UIElementBase : BaseObject
    {
        public bool Hovered = false;
        public override Vector2 Position { get => InternalPosition; set => InternalPosition = value; }

        /// <summary>
        /// Triggered once mouse enters.
        /// </summary>
        public virtual void HoverEnter()
        {
        }

        /// <summary>
        /// Triggered once mouse leaves.
        /// </summary>
        public virtual void HoverLeave()
        {
        }

        /// <summary>
        /// Triggered every frame while the mouse hovers.
        /// </summary>
        public virtual void HoverStay()
        {
        }

        protected static void DrawBorder(char[] screen, int screenWidth, char borderchar)
        {
            for (int i = 0; i < screen.Length; i++)
            {
                if (i % screenWidth == 0 || i <= screenWidth || screen.Length - i <= screenWidth || i % screenWidth == screenWidth - 1)
                {
                    screen[i] = borderchar;
                }
            }
        }
    }
}
