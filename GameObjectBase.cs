﻿namespace RPGEngine2
{
    public abstract class GameObjectBase : BaseObject
    {
        /// <summary>
        /// Translation applied every realtime-second.
        /// </summary>
        public Vector2 Velocity;
        public override Vector2 Position { get => InternalPosition; set => InternalPosition = value; }
    }
}