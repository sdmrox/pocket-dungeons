using UnityEngine;

namespace PocketDungeons.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Remap a value from one range to another.
        /// </summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float t = Mathf.InverseLerp(fromMin, fromMax, value);
            return Mathf.Lerp(toMin, toMax, t);
        }

        /// <summary>
        /// Weighted random selection. Returns the index of the chosen entry.
        /// </summary>
        public static int WeightedRandom(float[] weights)
        {
            float total = 0f;
            foreach (float w in weights)
                total += w;

            float roll = Random.Range(0f, total);
            float cumulative = 0f;

            for (int i = 0; i < weights.Length; i++)
            {
                cumulative += weights[i];
                if (roll <= cumulative)
                    return i;
            }

            return weights.Length - 1;
        }

        /// <summary>
        /// Exponential decay for screen shake, knockback, etc.
        /// </summary>
        public static float ExponentialDecay(float value, float decayRate, float deltaTime)
        {
            return value * Mathf.Exp(-decayRate * deltaTime);
        }
    }
}
