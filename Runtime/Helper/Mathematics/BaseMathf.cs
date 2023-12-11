using System;
using Base.Logging;
using UnityEngine;

namespace Base.Helper
{
    /// <summary>
    /// Provides extension methods for math operations.
    /// </summary>
    public static class BaseMathf
    {
        /// <summary>
        /// Returns the inversed value. This means a positive value
        /// if the given value is negative and negative value if the 
        /// given one is positive.
        /// </summary>
        /// <param name="value">The value to inverse.</param>
        /// <returns>The inversed value.</returns>
        public static int Inverse(this int value) => value * -1;

        /// <summary>
        /// Returns the inversed value. This means a positive value
        /// if the given value is negative and negative value if the 
        /// given one is positive.
        /// </summary>
        /// <param name="value">The value to inverse.</param>
        /// <returns>The inversed value.</returns>
        public static double Inverse(this double value) => value *= -1d;

        /// <summary>
        /// Returns the inversed value so a positive value
        /// if this one is negative and negative if this one is positive.
        /// </summary>
        /// <param name="value">The value to inverse.</param>
        /// <returns>The inversed value.</returns>
        public static float Inverse(this float value) => value * -1f;

        /// <summary>
        /// Returns the complement of the value so (1 - 'value').
        /// </summary>
        /// <param name="value">The value to get the complement of.</param>
        /// <returns>The complement.</returns>
        public static float Complement(this float value)
        {
            if (value < 0.0f || value > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(value), "Expects value between in range 0 to 1.");

            return 1.0f - value;
        }

        /// <summary>
        /// Returns the complement of the value so (1 - 'value').
        /// </summary>
        /// <param name="value">The value to get the complement of.</param>
        /// <returns>The complement.</returns>
        public static double Complement(this double value)
        {
            if (value < 0.0d || value > 1.0d)
                throw new ArgumentOutOfRangeException(nameof(value), "Expects value between in range 0 to 1.");

            return 1.0d - value;
        }

        /// <summary>
        /// Returns whether the value is greater than or equal to a minimal value 
        /// and smaller than or equal to a maximum value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimal value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="inclusive">Determine inclusive or exclusive</param>
        /// <returns>Whether the value is in the range.</returns>
        public static bool InRange(this int value, int min, int max, bool inclusive = false)
        {
            if (min > max)
            {
                PDebug.WarnFormat("[BaseMathf] The range value are inappropriate, this function will return false by default: min({0}) - max({1})", min, max);
                return false;
            }

            return inclusive ? value >= min && value <= max : value > min && value < max;
        }

        /// <summary>
        /// Returns the normalized (between 0 and 1) value.
        /// </summary>
        /// <param name="value">The value to normalize.</param>
        /// <param name="min">The minimum value to use.</param>
        /// <param name="max">The maximum value to use.</param>
        /// <returns>The normalized value.</returns>
        public static float Normalize(this float value, float min, float max) => (value - min) / (max - min);

        /// <summary>
        /// Returns the value mapped to a new scale.
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="min">The minimum range.</param>
        /// <param name="max">The maximum range.</param>
        /// <param name="targetMin">The new minimum range.</param>
        /// <param name="targetMax">The new maximum range.</param>
        /// <returns>The mapped value.</returns>
        public static float Map(this float value, float min, float max, float targetMin, float targetMax)
                        => (value - min) * ((targetMax - targetMin) / (max - min)) + targetMin;
        
        /// <summary>
        /// Calculate the ballistic vector of 2 position with predefined angle
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 CalcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
        {
            Vector3 direction = target - source;
            float h = direction.y;
            direction.y = 0;
            float distance = direction.magnitude;
            float a = angle * Mathf.Deg2Rad;
            direction.y = distance * Mathf.Tan(a);
            distance += h / Mathf.Tan(a);

            // calculate velocity
            float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
            return velocity * direction.normalized;
        }
    }
}