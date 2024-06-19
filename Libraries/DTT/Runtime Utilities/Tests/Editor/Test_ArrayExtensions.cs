#if TEST_FRAMEWORK

using DTT.Utils.Extensions;
using NUnit.Framework;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="ArrayExtensions"/> class.
    /// </summary>

    public class Test_ArrayExtensions
    {
        /// <summary>
        /// Tests whether the check on whether an index is in bounds of an array is correct.
        /// It expects an index in bounds of the array to return true. 
        /// </summary>
        [Test]
        public void Test_HasIndex_Correct([Values(0, 1, 2, 3, 4)] int index)
        {
            // Arrange.
            int[] array = new int[5];

            // Act.
            bool condition = array.HasIndex(index);

            // Assert.
            Assert.IsTrue(condition, "Expected the index to be inside the bounds of the array but it wasn't.");
        }

        /// <summary>
        /// Tests whether the check on whether an index is in bounds of an array is correct.
        /// It expects an index that is greater or equal to the array's length to return false.
        /// </summary>
        [Test]
        public void Test_HasIndex_Incorrect_ToHigh([Values(5, int.MaxValue)] int index)
        {
            // Arrange.
            int[] array = new int[5];

            // Act.
            bool condition = array.HasIndex(index);

            // Assert.
            Assert.IsFalse(condition, "Expected the index to be out of bounds of the array but it wasn't.");
        }

        /// <summary>
        /// Tests whether the check on whether an index is in bounds of an array is correct.
        /// It expects an index that is smaller than zero to return false.
        /// </summary>
        [Test]
        public void Test_HasIndex_Incorrect_ToLow([Values(-55, -int.MaxValue)] int index)
        {
            // Arrange.
            int[] array = new int[5];

            // Act.
            bool condition = array.HasIndex(index);

            // Assert.
            Assert.IsFalse(condition, "Expected the index to be outside the bounds of the array but it wasn't.");
        }
    }
}

#endif