
namespace RPGEngine2
{
    public class Particle : GameObjectBase
    {
        protected bool[,] InitialState;
        protected bool[,] CurrentState;
        protected int TickDuration;
        /// <summary>
        /// The animation repeats.
        /// </summary>
        protected bool isLooping;
        /// <summary>
        /// Character that represents alive-cells.
        /// </summary>
        protected char CellAlive;
        /// <summary>
        /// Character that represents dead-cells. Typically ' ' or '\0' (for transparency).
        /// </summary>
        protected char CellDead;
        /// TODO: Replace frame based animation with real-time based animation.
        protected int FrameCount = -1;
        protected int FramesPerTick;

        protected Vector2 PositionOffset;

        public override Vector2 Position 
        {
            get => InternalPosition - PositionOffset;
            set => InternalPosition = value + PositionOffset; 
        }

        /// <summary>
        /// Called when the animation is finished, isn't triggered when <c>isLooping</c> is true. Called <i>just</i> before <c>Destroy()</c> is called.
        /// </summary>
        public virtual void OnFinished()
        {
        }

        public override void Update()
        {
            FrameCount++;

            if (FrameCount % FramesPerTick != 0)
                return;

            if (FrameCount / FramesPerTick == TickDuration)
            {
                if (isLooping)
                {
                    CurrentState = (bool[,])InitialState.Clone();
                    FrameCount = -1;
                }
                else
                {
                    OnFinished();
                    Destroy();
                }

                return;
            }
            
            if (NeedsExpanding(CurrentState))
            {
                CurrentState = ExpandRectArray(CurrentState, 1);
                Size = new Vector2(CurrentState.GetLength(0), CurrentState.GetLength(1));
                PositionOffset -= Vector2.One;
                InternalPosition -= Vector2.One;
            }

            bool[,] nextState = new bool[CurrentState.GetLength(0), CurrentState.GetLength(1)];

            for (int x = 0; x < nextState.GetLength(0); x++)
            {
                for (int y = 0; y < nextState.GetLength(1); y++)
                {
                    int livingCells = 0;
                    foreach (var item in new Vector2(x, y).Neighbours())
                    {
                        if (item.x < 0 || item.y < 0 || item.RoundX >= nextState.GetLength(0) || item.RoundY >= nextState.GetLength(1))
                            continue;

                        if (CurrentState[item.RoundX, item.RoundY] == true)
                            livingCells++;
                    }

                    // 
                    if (CurrentState[x, y] == true)
                    {
                        if (livingCells < 2 || livingCells > 3)
                        {
                            // ingen grund til at assign når den er false som standard
                            //nextState[x, y] = false;
                            continue;
                        }
                    }
                    // Dead celle bliver levende
                    else if (livingCells == 3)
                    {
                        nextState[x, y] = true;
                        continue;
                    }

                    nextState[x, y] = CurrentState[x, y];
                }
            }

            CurrentState = nextState;
            //FrameCount++;
        }

        public override void Render()
        {
            RecentRendered = new char[CurrentState.Length];

            for (int y = 0; y < CurrentState.GetLength(1); y++)
            {
                for (int x = 0; x < CurrentState.GetLength(0); x++)
                {
                    RecentRendered[y * Size.RoundX + x] = CurrentState[x, y] ? CellAlive : CellDead;
                }
            }
        }

        /// <summary>
        /// Checks whether the current array has <c>true</c> in any of the borders.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static bool NeedsExpanding(bool[,] state)
        {
            for (int x = 0; x < state.GetLength(0); x++)
            {
                for (int y = 0; y < state.GetLength(1); y++)
                {
                    if ((x > 0 && x < state.GetLength(0) - 1) && (y > 0 && y < state.GetLength(1) - 1))
                        continue;

                    if (state[x, y] == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Expands a rectangular array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="additionalSize"></param>
        /// <returns></returns>
        private static bool[,] ExpandRectArray(bool[,] array, int additionalSize)
        {
            bool[,] newArray = new bool[array.GetLength(0) + additionalSize * 2, array.GetLength(1) + additionalSize * 2];

            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    newArray[x + additionalSize, y + additionalSize] = array[x, y];
                }
            }

            return newArray;
        }
    }
}
