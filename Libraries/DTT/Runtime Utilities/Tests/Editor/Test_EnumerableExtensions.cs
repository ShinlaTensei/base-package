#if TEST_FRAMEWORK

using DTT.Utils.Extensions;
using NUnit.Framework;
using System.Collections.Generic;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="EnumerableExtensions"/> class.
    /// </summary>
    public class Test_EnumerableExtensions
    {
        /// <summary>
        /// Tests whether an enumerable can be correctly declared as null or empty.
        /// It expects the list to be null or empty if no entries are inside.
        /// </summary>
        [Test]
        public void Test_IsNullOrEmpty_True_Empty()
        {
            // Arrange.
            List<int> list = new List<int>();

            // Act.
            bool condition = list.IsNullOrEmpty();

            // Assert.
            Assert.IsTrue(condition, "Expected the empty list to be conditioned as null or empty but it wasn't.");
        }

        /// <summary>
        /// Tests whether an enumerable can be correctly declared as null or empty.
        /// It expects the list to be null or empty if the list is null.
        /// </summary>
        [Test]
        public void Test_IsNullOrEmpty_True_Null()
        {
            // Arrange.
            List<int> list = null;

            // Act.
            bool condition = list.IsNullOrEmpty();

            // Assert.
            Assert.IsTrue(condition, "Expected the null list to be conditioned as null or empty but it wasn't.");
        }

        /// <summary>
        /// Tests whether an enumerable can be correctly declared as null or empty.
        /// It expects the list not to be null or empty if entries are inside.
        /// </summary>
        [Test]
        public void Test_IsNullOrEmpty_False()
        {
            // Arrange.
            List<int> list = new List<int> { 0 };

            // Act.
            bool condition = list.IsNullOrEmpty();

            // Assert.
            Assert.IsFalse(condition, "Expected the list with an entry to not be conditioned as null or empty but it was.");
        }
    }
}

#endif