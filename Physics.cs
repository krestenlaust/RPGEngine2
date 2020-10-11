using System.Collections.Generic;
using System.Linq;

namespace RPGEngine2
{
    // TODO: Add more physics features.
    public static class Physics
    {
        public static void GameObjectPhysics(List<GameObjectBase> physicsObjects)
        {
            foreach (GameObjectBase thisObj in physicsObjects)
            {
                List<GameObjectBase> collidingObjects = (from obj in physicsObjects
                                                         where Vector2.RectCollide(thisObj.InternalPosition, thisObj.Size, obj.Position, obj.Size)
                                                         select obj).ToList();

                if (collidingObjects.Count > 0)
                {
                    thisObj.Collision(collidingObjects);
                }
            }
        }
    }
}
