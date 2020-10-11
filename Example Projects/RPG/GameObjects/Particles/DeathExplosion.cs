using RPGEngine2;

namespace RPG.GameObjects.Particles
{
    public class DeathExplosion : Particle
    {
        public DeathExplosion(Vector2 position)
        {
            InitialState = new bool[5, 5];
            InitialState[2, 0] = true;
            InitialState[2, 4] = true;
            InitialState[0, 2] = true;
            InitialState[4, 2] = true;
            InitialState[1, 1] = true;
            InitialState[1, 2] = true;
            InitialState[1, 3] = true;
            InitialState[3, 1] = true;
            InitialState[3, 2] = true;
            InitialState[3, 3] = true;

            CurrentState = InitialState;

            //TickDuration = 3;
            TickInterval = 0.1f;
            //FramesPerTick = 6;
            AnimationDurationTicks = 7;

            PositionOffset = new Vector2(-2, -2);
            Size = new Vector2(5, 5);
            isLooping = false;
            Position = position;
            CellAlive = '*';
            CellDead = '.';
        }
    }
}
