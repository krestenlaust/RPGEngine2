namespace RPGEngine2
{
    public class GameConfig
    {
        /// <summary>
        /// The max amount of frames per second. 0 is no-limit.
        /// </summary>
        public int FramesPerSecondCap;
        /// <summary>
        /// The target framerate of physics/fixedupdate.
        /// </summary>
        public int PhysicsUpdateRate;
        /// <summary>
        /// The interval in seconds the console title text updates.
        /// </summary>
        public float ConsoleTitleRefreshRate;
        /// <summary>
        /// If true, then the character '\0' won't over-write previously written characters — effectively making them transparent.
        /// </summary>
        public bool isNullSeeThrough;

        /// <summary>
        /// Initializes with default settings.
        /// </summary>
        public GameConfig()
        {
            FramesPerSecondCap = 0;
            PhysicsUpdateRate = 30;
            ConsoleTitleRefreshRate = 0.1f;
            isNullSeeThrough = true;
        }
    }
}
