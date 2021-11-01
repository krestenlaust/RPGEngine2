﻿using RPGEngine2;
using static RPG.GameCode;

namespace RPG
{
    public class RocketExplosion : Particle
    {
        public int Damage = 15;

        public RocketExplosion(Vector2 position)
        {
            // Prepares conways particle.
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

            //TickDuration = 7; // 8
            //FramesPerTick = 3;
            TickInterval = 0.05f;
            AnimationDurationTicks = 7;

            PositionOffset = new Vector2(-2, -2);
            Size = new Vector2(5, 5);
            isLooping = false;
            Position = position;
            CellAlive = '#';
            CellDead = '\0';
        }

        public override void OnFinished()
        {
            Vector2 truePosition = Position + PositionOffset;

            foreach (var item in Enemies)
            {
                if (Vector2.RectCollide(truePosition, Size, item.Position, item.Size))
                {
                    item.HP -= Damage;
                }
            }
            Controller.SetVibration(Rocket.CONTROLLER_RUMBLE / 2, 0, PlayerControllerID);
            BombsAlive--;

            base.OnFinished();
        }
    }
}
