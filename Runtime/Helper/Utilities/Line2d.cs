#region Header
// Date: 12/10/2023
// Created by: Huynh Phong Tran
// File name: Line2d.cs
#endregion

using System;
using UnityEngine;
using static UnityEngine.Mathf;
using static UnityEngine.Vector2;

namespace Base.Helper
{
    /// <summary>
    /// Describes a line in two-dimensional space
    /// </summary>
    [Serializable]
    public struct Line2d
    {
        // Note:
        // This was written as a mathematical helper for some 2d operations.
        // Most of the mathematical functions are missing, so if you need them, feel free to add them here, to make this struct more universally functional.

        [SerializeField] private float _distance;
        [SerializeField] private Vector2 _normal; // should always be normalized!

        /// <summary>
        /// Create a new Line from two points
        /// </summary>
        public Line2d(Vector2 p1, Vector2 p2)
        {
            _normal = new Vector2(p1.y - p2.y, p2.x - p1.x);
            if (Abs(_normal.sqrMagnitude - 1f) > Epsilon)
            {
                _normal.Normalize();
            }

            _distance = Dot(p1, _normal);
        }

        /// <summary>
        /// Create a line from normal and distance
        /// </summary>
        public Line2d(Vector2 normal, float distance)
        {
            if (Abs(normal.sqrMagnitude - 1f) > Epsilon)
            {
                normal.Normalize();
            }

            _normal = normal;
            _distance = distance;
        }

        /// <summary>
        /// The Distance from (0,0)
        /// </summary>
        public float Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        /// <summary>
        /// The Line's normal. Always normalized. Multiplied by distance gives the closest point to (0,0)
        /// </summary>
        public Vector2 Normal
        {
            get { return _normal; }
            set
            {
                _normal = value;
                if (Abs(_normal.sqrMagnitude - 1f) > Epsilon)
                {
                    _normal.Normalize();
                }
            }
        }

        /// <summary>
        /// Calculates the distance a point has from the line. The sign signifies on which side of the line the point is
        /// </summary>
        /// <param name="point">the point</param>
        /// <param name="line">the line</param>
        /// <returns>The distance. if positive the point is in the direction of the normal, if negative on the other side</returns>
        public static float SignedDistance(Vector2 point, Line2d line)
        {
            return Dot(point, line._normal) - line._distance;
        }

        /// <summary>
        /// Inverts the direction of the Line. It runs through the same points as it's inverted counterpart, but has an inverted normal (and therefore inverted distance)
        /// </summary>
        public void Invert()
        {
            _distance *= -1f;
            _normal *= -1f;
        }

        public Line2d Inverted
        {
            get { return new Line2d(_normal * -1f, _distance * -1f); }
        }

        /// <summary>
        /// Interpolates two lines. Makes sure no line flips in the middle!
        /// </summary>
        public static Line2d Lerp(Line2d l1, Line2d l2, float t)
        {
            if (Dot(l1._normal, l2._normal) < 0f)
            {
                l2.Invert();
            }

            return TrueLerp(l1, l2, t);
        }

        /// <summary>
        /// Interpolates two lines. Ignores if the normals are opposing each other, making rotations up to 180° possible!
        /// </summary>
        public static Line2d TrueLerp(Line2d l1, Line2d l2, float t)
        {
            return new Line2d(Vector2.Lerp(l1._normal, l2._normal, t), Mathf.Lerp(l1._distance, l2._distance, t));
        }

        /// <summary>
        /// Returns the point where two lines collide. Returns null if they are parallel
        /// </summary>
        public static Optional<Vector2> Collision(Line2d l1, Line2d l2)
        {
            // lines run parallel
            if (l1.IsParallel(l2))
            {
                return Optional<Vector2>.None();
            }

            // Copied from Wikipedia
            return new Vector2
            (
                (l1._distance * l2._normal.y - l2._distance * l1._normal.y) /
                (l1._normal.x * l2._normal.y - l2._normal.x * l1._normal.y),
                (l1._normal.x * l2._distance - l2._normal.x * l1._distance) /
                (l1._normal.x * l2._normal.y - l2._normal.x * l1._normal.y)
            );
        }

        public bool IsParallel(Line2d l2)
        {
            return _normal == l2._normal || _normal == -l2._normal;
        }

        /// <summary>
        /// Outputs the point where two lines collide
        /// Can fail if lines are parallel
        /// </summary>
        /// <returns>The point where the lines collide</returns>
        public Optional<Vector2> Collision(Line2d l2) => Collision(this, l2);


        [Obsolete("Use Optional<Vector2> Collision instead")]
        public static bool TryCollision(Line2d l1, Line2d l2, out Vector2 result)
        {
            Optional<Vector2> potentialCollisionPoint = Collision(l1, l2);
            if (!potentialCollisionPoint.TryGetSome(out result))
            {
                result = new Vector2();
                return false;
            }
            return true;
        }
    }
}