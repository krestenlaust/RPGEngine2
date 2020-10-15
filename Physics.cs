using System.Collections.Generic;
using System.Linq;

namespace RPGEngine2
{
    // TODO: Add more physics features.
    public static class Physics
    {
        public static void GameObjectPhysics(List<GameObjectBase> physicsObjects)
        {
            if (physicsObjects is null)
                return;

            //HashSet<GameObjectBase> checkedObjects = new HashSet<GameObjectBase>();

            // TODO: tror måske man kunne lave noget smart med en slags løbende liste som bliver kortere, måske en stack hvor man peeker mest og så fjerner bagefter.

            foreach (GameObjectBase thisObj in physicsObjects)
            {
                foreach (var otherObj in physicsObjects)
                {
                    if (otherObj == thisObj)
                        continue;

                    if (!Vector2.RectCollide(thisObj.InternalPosition, thisObj.Size, otherObj.InternalPosition, otherObj.Size))
                        continue;

                    thisObj.Collision(otherObj);
                    // TODO: Optimize this, there is no reason in checking both, but if I do this then I'll call the method twice for every object colliding.
                    //otherObj.Collision(thisObj);
                }
            }
        }
    }
}
