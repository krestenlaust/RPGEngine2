using System.Collections;
using System.Collections.Generic;

namespace RPGEngine2
{
    public abstract class GameObjectBase : BaseObject
    {
        /// <summary>
        /// Translation applied every real-time second.
        /// </summary>
        public Vector2 Velocity;
        //public override Vector2 Position { get => InternalPosition; set => InternalPosition = value; }
        protected Vector2 PositionOffset;
        public bool PhysicsEnabled = false;

        public override Vector2 Position
        {
            get => InternalPosition - PositionOffset;
            set => InternalPosition = value + PositionOffset;
        }

        public virtual void Collision(List<GameObjectBase> gameObjects)
        {
        }
    }
}
