using UnityEngine;

namespace PocketDungeons.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Set only the X component of a Vector3.
        /// </summary>
        public static Vector3 WithX(this Vector3 v, float x) => new(x, v.y, v.z);

        /// <summary>
        /// Set only the Y component of a Vector3.
        /// </summary>
        public static Vector3 WithY(this Vector3 v, float y) => new(v.x, y, v.z);

        /// <summary>
        /// Shuffle an array in-place using Fisher-Yates.
        /// </summary>
        public static void Shuffle<T>(this T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        /// <summary>
        /// Check if a layer is in a LayerMask.
        /// </summary>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }
}
