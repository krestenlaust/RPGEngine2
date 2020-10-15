using System.Collections.Generic;
using static RPGEngine2.EngineMain;

namespace RPGEngine2
{
    // TODO: Needs proper profiling and improved performance.
    public abstract class Particle : GameObjectBase
    {
        private static readonly Dictionary<bool[,], bool[,]> conwaysCacheOfLife = new Dictionary<bool[,], bool[,]>();
        /// <summary>
        /// The duration in ticks.
        /// </summary>
        public int AnimationDurationTicks { get; protected set; }
        public bool[,] InitialState { get; protected set; }
        public bool[,] CurrentState { get; protected set; }
        /// <summary>
        /// The animation repeats.
        /// </summary>
        public bool isLooping { get; protected set; }
        /// <summary>
        /// Character that represents alive-cells.
        /// </summary>
        public char CellAlive { get; set; }
        /// <summary>
        /// Character that represents dead-cells. Typically ' ' or '\0' (for transparency).
        /// </summary>
        public char CellDead { get; set; }
        protected float TickInterval;
        private float deltaTickTime;
        private int tickCount;

        /// <summary>
        /// The duration in seconds.
        /// </summary>
        public float AnimationDurationSeconds
        {
            get => AnimationDurationTicks * TickInterval;
        }

        /// <summary>
        /// NOTE: Call base implementation for destruction of particle after animation. Called when the animation is finished, isn't triggered when <c>isLooping</c> is true. Called <i>just</i> before <c>Destroy()</c> is called.
        /// </summary>
        public virtual void OnFinished()
        {
            Destroy();
        }

        public override void Update()
        {
            deltaTickTime += DeltaTime;

            if (deltaTickTime < TickInterval)
                return;

            deltaTickTime -= TickInterval;

            if (tickCount++ >= AnimationDurationTicks)
            {
                if (isLooping)
                {
                    CurrentState = (bool[,])InitialState.Clone();
                    tickCount = 0;
                }
                else
                {
                    OnFinished();
                    return;
                }
            }

            if (NeedsExpanding(CurrentState))
            {
                CurrentState = ExpandRectArray(CurrentState, 1);
                Size = new Vector2(CurrentState.GetLength(0), CurrentState.GetLength(1));
                PositionOffset -= Vector2.One;
                InternalPosition -= Vector2.One;
            }

            CurrentState = CalculateNextState(CurrentState);
        }

        public override char[] Render()
        {
            char[] render = new char[CurrentState.Length];

            for (int y = 0; y < CurrentState.GetLength(1); y++)
            {
                for (int x = 0; x < CurrentState.GetLength(0); x++)
                {
                    render[y * Size.RoundX + x] = CurrentState[x, y] ? CellAlive : CellDead;
                }
            }

            return render;
        }

        private static bool[,] CalculateNextState(bool[,] state)
        {

            if (conwaysCacheOfLife.TryGetValue(state, out bool[,] nextState))
            {
                return (bool[,])nextState.Clone();
            }

            nextState = new bool[state.GetLength(0), state.GetLength(1)];

            for (int x = 0; x < nextState.GetLength(0); x++)
            {
                for (int y = 0; y < nextState.GetLength(1); y++)
                {
                    int livingCells = 0;
                    foreach (var item in new Vector2(x, y).Neighbours())
                    {
                        if (item.x < 0 || item.y < 0 || item.RoundX >= nextState.GetLength(0) || item.RoundY >= nextState.GetLength(1))
                            continue;

                        if (state[item.RoundX, item.RoundY] == true)
                            livingCells++;
                    }

                    if (state[x, y] == true)
                    {
                        if (livingCells < 2 || livingCells > 3)
                        {
                            // no point in assigning false when it's false by default.
                            continue;
                        }
                    }
                    // Dead cell is vitalized.
                    else if (livingCells == 3)
                    {
                        nextState[x, y] = true;
                        continue;
                    }

                    nextState[x, y] = state[x, y];
                }
            }

            conwaysCacheOfLife[(bool[,])state.Clone()] = (bool[,])nextState.Clone();

            return nextState;
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
