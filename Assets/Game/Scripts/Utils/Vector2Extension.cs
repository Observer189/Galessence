using UnityEngine;

namespace Game.Scripts.Utils
{
    public static class Vector2Extension
    {
        public static float ProjectionTo(this Vector2 vector, Vector2 projectionTo)
        {
            return (vector.x * projectionTo.x + vector.y * projectionTo.y) / projectionTo.magnitude;
        }
    }
}