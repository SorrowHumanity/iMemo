namespace Assets.Scripts.GameCategory.NumberGames
{
    using UnityEngine;

    public static class PhysicsUtil
    {
        /// <summary>
        ///     Checks if a vector overlaps nearby colliders.
        /// </summary>
        /// <param name="position"> The position vector. </param>
        /// <param name="bounds"> The bounds. </param>
        /// <returns> true, if the vector does not overlap. Otherwise, false. </returns>
        public static bool CheckBounds2D(Vector2 position, Vector2 bounds)
        {
            Bounds boxBounds = new Bounds(position, bounds);

            float sqrHalfBoxSize = boxBounds.extents.sqrMagnitude;
            float overlapingCircleRadius = Mathf.Sqrt(sqrHalfBoxSize + sqrHalfBoxSize);
            
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, overlapingCircleRadius, -1);
            foreach (Collider2D otherCollider in hitColliders)
            {
                if (otherCollider.bounds.Intersects(boxBounds))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
