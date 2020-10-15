namespace RPGEngine2
{
    /// <summary>
    /// A quick and dirty way to debug a value. Is automatically instantiated. NOTE: Remember to enable DebugStrings in the renderer.
    /// </summary>
    public class DebugString
    {
        internal char[] message;
        public Vector2 Position;
        
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

        public void SetMessage(char[] message) => this.message = message;
        public void SetMessage(string message)
        {
            if (message is null)
            {
                SetMessage("null");
                return;
            }

            SetMessage(message.ToString());
        }
        public void SetMessage(object message)
        {
            if (message is null)
            {
                SetMessage("null");
                return;
            }

            SetMessage(message.ToString());    
        }

        public string GetMessage() => message.ToString();

        public void Destroy() => Renderer.DebugStrings.Remove(this);
    }
}
