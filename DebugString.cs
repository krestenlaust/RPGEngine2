namespace RPGEngine2
{
    /// <summary>
    /// A quick and dirty way to debug a value. Is automatically instantiated. 
    /// NOTE: Remember to enable DebugStrings in the renderer.
    /// </summary>
    public class DebugString
    {
        /// <summary>
        /// The position of the debug string.
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// The message that is displayed. Change using <c>SetMessage</c>.
        /// </summary>
        internal char[] message;

        public DebugString(Vector2 position)
        {
            Position = position;
        }

        public DebugString(Vector2 position, char[] message)
        {
            this.message = message;
            Position = position;
            Renderer.DebugStrings.Add(this);
        }

        /// <summary>
        /// Updates debug string with char array.
        /// </summary>
        /// <param name="message"></param>
        public void SetMessage(char[] message) => this.message = message;

        /// <summary>
        /// Updates debug string with new string.
        /// </summary>
        /// <param name="message"></param>
        public void SetMessage(string message)
        {
            if (message is null)
            {
                SetMessage("null");
                return;
            }

            /// Changed from message.ToString to message.ToCharArray, to prevent possible recursion?.
            SetMessage(message.ToCharArray());
        }

        /// <summary>
        /// Updates debug string with object string-value.
        /// </summary>
        /// <param name="message"></param>
        public void SetMessage(object message)
        {
            if (message is null)
            {
                SetMessage("null");
                return;
            }

            SetMessage(message.ToString());
        }

        /// <summary>
        /// Returns the currently displayed message as string.
        /// </summary>
        /// <returns></returns>
        public string GetMessage() => message.ToString();

        /// <summary>
        /// Destroys the <c>DebugString</c> object, by removing it from <c>Renderer.DebugStrings</c>.
        /// </summary>
        public void Destroy() => Renderer.DebugStrings.Remove(this);
    }
}
