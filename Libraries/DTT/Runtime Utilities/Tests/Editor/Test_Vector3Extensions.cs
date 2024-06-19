#if TEST_FRAMEWORK

using DTT.Utils.Extensions;
using NUnit.Framework;
using UnityEngine;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Holds tests for the vector3 extensions class.
    /// </summary>
    public class Test_Vector3Extensions
    {
        /// <summary>
        /// Tests whether the resulting vector of a flatten operation has the correct
        /// axis set to 0.
        /// </summary>
        /// <param name="axis">The axis to flatten.</param>
        /// <param name="expectedX">The expected x value.</param>
        /// <param name="expectedY">The expected y value.</param>
        /// <param name="expectedZ">The expected z value.</param>
        [TestCase(Vector3Axis.X | Vector3Axis.Y | Vector3Axis.Z, 0f, 0f, 0f)]
        [TestCase(Vector3Axis.Y | Vector3Axis.Z, 1f, 0,0f)]
        [TestCase(Vector3Axis.X | Vector3Axis.Z, 0f, 1f, 0f)]
        [TestCase(Vector3Axis.X | Vector3Axis.Y, 0f, 0f, 1f)]
        [TestCase(Vector3Axis.Z, 1f, 1f, 0f)]
        [TestCase(Vector3Axis.Y, 1f, 0f, 1f)]
        [TestCase(Vector3Axis.X, 0f, 1f, 1f)]
        [Test]
        public void Test_Flatten(Vector3Axis axis, float expectedX, float expectedY, float expectedZ)
        {
            // Arrange.
            Vector3 initial = Vector3.one;
            
            // Act.
            Vector3 result = initial.Flatten(axis);
            Vector3 expected = new Vector3(expectedX, expectedY, expectedZ);
            
            // Assert.
            Assert.AreEqual(expected, result);
        }
    }
}

#endif
