#if TEST_FRAMEWORK

using DTT.Utils.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="MathExtensions"/> class.
    /// </summary>
    public class Test_MathExtensions
    {
        /// <summary>
        /// An array with lists of values used for testing the 'Map' method where the
        /// values correspond as follows: [value, min, max, newMin, newMax, expected].
        /// </summary>
        private static readonly List<int>[] _mappables = new List<int>[]
        {
            new List<int>{ 5, 0, 10, 10, 20, 15 },
            new List<int>{ 0, -5, 5, 10, 20, 15 },
            new List<int>{ -5, -10, 0, 10, 20, 15 },
            new List<int>{ 5, 0, 10, -20, -10, -15 }
        };

        /// <summary>
        /// Tests whether the inverse of a value can be correctly done.
        /// It expects a positive float to return a negative inverse.
        /// </summary>
        [Test]
        public void Test_Inverse_Float_Positive()
        {
            // Arrange.
            float value = 50f;

            // Act.
            float inverse = value.Inverse();

            // Assert.
            Assert.Negative(inverse, "Expected the inverse value to be a negative value but it wasn't.");
        }

        /// <summary>
        /// Tests whether the inverse of a value can be correctly done.
        /// It expects a negative float to return a positive inverse.
        /// </summary>
        [Test]
        public void Test_Inverse_Float_Negative()
        {
            // Arrange.
            float value = -50f;

            // Act.
            float inverse = value.Inverse();

            // Assert.
            Assert.Positive(inverse, "Expected the inverse value to be a positive value but it wasn't.");
        }

        /// <summary>
        /// Tests whether the inverse of a value can be correctly done.
        /// It expects a positive double to return a negative inverse.
        /// </summary>
        [Test]
        public void Test_Inverse_Double_Positive()
        {
            // Arrange.
            double value = 50d;

            // Act.
            double inverse = value.Inverse();

            // Assert.
            Assert.Negative(inverse, "Expected the inverse value to be a negative value but it wasn't.");
        }

        /// <summary>
        /// Tests whether the inverse of a value can be correctly done.
        /// It expects a negative double to return a positive inverse.
        /// </summary>
        [Test]
        public void Test_Inverse_Double_Negative()
        {
            // Arrange.
            double value = -50d;

            // Act.
            double inverse = value.Inverse();

            // Assert.
            Assert.Positive(inverse, "Expected the inverse value to be a positive value but it wasn't.");
        }

        /// <summary>
        /// Tests whether the inverse of a value can be correctly done.
        /// It expects a positive integer to return a negative inverse.
        /// </summary>
        [Test]
        public void Test_Inverse_Int_Positive()
        {
            // Arrange.
            int value = 50;

            // Act.
            int inverse = value.Inverse();

            // Assert.
            Assert.Negative(inverse, "Expected the inverse value to be a negative value but it wasn't.");
        }

        /// <summary>
        /// Tests whether the inverse of a value can be correctly done.
        /// It expects a negative integer to return a positive inverse.
        /// </summary>
        [Test]
        public void Test_Inverse_Int_Negative()
        {
            // Arrange.
            int value = -50;

            // Act.
            int inverse = value.Inverse();

            // Assert.
            Assert.Positive(inverse, "Expected the inverse value to be a positive value but it wasn't.");
        }

        /// <summary>
        /// Tests whether the complement can be given of a value between 0 and 1.
        /// It expects the complement of a value between 0 and 1 to be equal to '1 - value'.
        /// </summary>
        [Test]
        public void Test_Complement_Float()
        {
            // Arrange.
            float value = 0.25f;

            // Act.
            float complement = value.Complement();
            float expected = 0.75f;

            // Assert.
            Assert.AreEqual(expected, complement, "Expected the complement to be 1 - 'value' but it wasn't.");
        }

        /// <summary>
        /// Tests whether the complement can be given of a value between 0 and 1.
        /// It expects an <see cref="ArgumentOutOfRangeException"/> to be thrown if the value is not within the 0-1 range.
        /// </summary>
        [Test]
        public void Test_Complement_Float_Out_Of_Range()
        {
            // Arrange.
            float value = 5f;

            // Act.
            TestDelegate action = () => value.Complement();

            // Assert.
            Assert.Catch<ArgumentOutOfRangeException>(action, "Expected the out of range argument to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether the complement can be given of a value between 0 and 1.
        /// It expects the complement of a value between 0 and 1 to be equal to '1 - value'.
        /// </summary>
        [Test]
        public void Test_Complement_Double()
        {
            // Arrange.
            double value = 0.25d;

            // Act.
            double complement = value.Complement();
            double expected = 0.75d;

            // Assert.
            Assert.AreEqual(expected, complement, "Expected the complement to be 1 - 'value' but it wasn't.");
        }

        /// <summary>
        /// Tests whether the complement can be given of a value between 0 and 1.
        /// It expects an <see cref="ArgumentOutOfRangeException"/> to be thrown if the value is not within the 0-1 range.
        /// </summary>
        [Test]
        public void Test_Complement_Double_Out_Of_Range()
        {
            // Arrange.
            double value = 5d;

            // Act.
            TestDelegate action = () => value.Complement();

            // Assert.
            Assert.Catch<ArgumentOutOfRangeException>(action, "Expected the out of range argument to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether a value can be normalized.
        /// It expects the normalized value to be a value between 0-1 based of the min and max value.
        /// </summary>
        [Test]
        public void Test_Normalize()
        {
            // Arrange.
            float value = 25f;

            // Act.
            float normalized = value.Normalize(0.0f, 100f);
            float expected = 0.25f;

            // Assert.
            Assert.AreEqual(expected, normalized, "Expected the value to be normalized but it wasn't.");
        }

        /// <summary>
        /// Tests whether a value can be mapped to a new scale.
        /// It expects the value to be mapped based on the old and new min/max values.
        /// </summary>
        [Test]
        public void Test_Map([ValueSource(nameof(_mappables))] List<int> values)
        {
            // Arrange.
            float value = values[0];

            // Act.
            float mapped = value.Map(values[1], values[2], values[3], values[4]);
            float expected = values[5];

            // Assert.
            Assert.AreEqual(expected, mapped, "Expected the value to be mapped to the new scale but it wasn't.");
        }
    }
}

#endif