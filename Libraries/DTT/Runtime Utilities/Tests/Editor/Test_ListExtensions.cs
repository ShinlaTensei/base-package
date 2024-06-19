#if TEST_FRAMEWORK

using DTT.Utils.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="ListExtensions"/> class.
    /// </summary>
    public class Test_ListExtensions
    {
        /// <summary>
        /// Holds an array with lists used for testing the null entry removal.
        /// </summary>
        private static readonly List<Object>[] _nullEntries = 
        {
            new List<Object>(),
            new List<Object>{ new Object(), null, new Object()},
            new List<Object>{ null, null, null },
            new List<Object>{ null, new Object(), null},
            new List<Object>{ new Object(), null, null},
            new List<Object>{ null, null, new Object() }
        };

        /// <summary>
        /// Holds an array with lists used for testing the default entry removal.
        /// </summary>
        private static readonly List<int>[] _defaultEntries = 
        {
            new List<int>(),
            new List<int>{ 5, 0, 5 },
            new List<int>{ 0, 0, 0 },
            new List<int>{ 0, 5, 0 },
            new List<int>{ 5,0, 0  },
            new List<int>{ 0, 0, 5 }
        };

        /// <summary>
        /// Tests whether null entries can be removed from a list.
        /// It expects all null entries to be removed.
        /// </summary>
        [Test]
        public void Test_RemoveNullEntries([ValueSource(nameof(_nullEntries))] List<Object> objects)
        {
            // Arrange.
            int nullEntryCount = objects.Count(obj => Equals(obj, null));
            int listCountWithNullEntries = objects.Count;

            // Act.
            objects.RemoveNullEntries();

            // Assert.
            Assert.AreEqual(listCountWithNullEntries - nullEntryCount, objects.Count, "Expected the null entries to be removed but they weren't.");
        }

        /// <summary>
        /// Tests whether default value entries can be removed from a list.
        /// It expects all default value entries to be removed.
        /// </summary>
        [Test]
        public void Test_RemoveDefaultEntries([ValueSource(nameof(_defaultEntries))] List<int> objects)
        {
            // Arrange.
            int defaultEntryCount = objects.Count(obj => Equals(obj, default(int)));
            int listCountWithDefaultEntries = objects.Count;

            // Act.
            objects.RemoveDefaultValues();

            // Assert.
            Assert.AreEqual(listCountWithDefaultEntries - defaultEntryCount, objects.Count, "Expected the default entries to be removed but they weren't.");
        }

        /// <summary>
        /// Tests whether the check on whether an index is in bounds of an array is correct.
        /// It expects an index in bounds of the array to return true. 
        /// </summary>
        [Test]
        public void Test_HasIndex_Correct()
        {
            // Arrange.
            int index  = 3;
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
        public void Test_HasIndex_Incorrect_TooHigh()
        {
            // Arrange.
            int index = 5;
            List<int> list = new List<int>(Enumerable.Range(0, 5));

            // Act.
            bool condition = list.HasIndex(index);

            // Assert.
            Assert.IsFalse(condition, "Expected the index to be out of bounds of the array but it wasn't.");
        }

        /// <summary>
        /// Tests whether the check on whether an index is in bounds of an array is correct.
        /// It expects an index that is smaller than zero to return false.
        /// </summary>
        [Test]
        public void Test_HasIndex_Incorrect_ToLow()
        {
            // Arrange.
            int index = -1;
            List<int> list = new List<int>(Enumerable.Range(0, 5));

            // Act.
            bool condition = list.HasIndex(index);

            // Assert.
            Assert.IsFalse(condition, "Expected the index to be outside the bounds of the array but it wasn't.");
        }
    }
}

#endif