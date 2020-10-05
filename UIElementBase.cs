namespace RPGEngine2
{
    public abstract class UIElementBase : BaseObject
    {
        public enum HoverState
        {
            None = 0,
            Enter = 1,
            Leave = 2,
            Stay = 3,
        }
        public HoverState CurrentHoverState;
        public bool Hovered { get
            {
                return CurrentHoverState == HoverState.Enter || CurrentHoverState == HoverState.Stay;
            } }
        public override Vector2 Position { get => InternalPosition; set => InternalPosition = value; }

        /// <summary>
        /// As an alternative to using <c>Update</c> method for checking hover state, this method is called when <c>CurrentHoverState</c> changes (NOTE: Only once even while hoverstate is stay)
        /// </summary>
        public virtual void HoverUpdate()
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
