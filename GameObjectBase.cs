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
        public bool PhysicsEnabled;

        /// <summary>
        /// Can be called multiple times per frame depending on how many objects are overlapping.
        /// </summary>
        /// <param name="gameObjects"></param>
        public virtual void Collision(GameObjectBase gameObjects)
        {
        }
    }
}
