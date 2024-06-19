#if TEST_FRAMEWORK

using DTT.Utils.Exceptions;
using DTT.Utils.Optimization;
using NUnit.Framework;
using System;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="LazyDictionaryBase{T}"/> class.
    /// </summary>
    public class Test_LazyDictionaryBase
    {
        /// <summary>
        /// A type safe cache with an unimplemented retrieval of a cached item.
        /// </summary>
        private class FailingCache : LazyDictionaryBase<string, int>
        {
            public int MyProperty => base[nameof(MyProperty)];

            protected override int GetValue(string nameOfProperty) => throw new NotImplementedException();
        }

        /// <summary>
        /// A type safe cached that only returns one cached value.
        /// </summary>
        private class SucceedingCache : LazyDictionaryBase<string, int>
        {
            public const int CACHED_VALUE = 5;

            public int MyProperty => base[nameof(MyProperty)];

            protected override int GetValue(string nameOfProperty) => CACHED_VALUE;
        }

        /// <summary>
        /// Tests whether property retrieval can be done properly and with feedback.
        /// It expects a <see cref="LazyDictionaryException"/> to be thrown if the retrieval failed.
        /// </summary>
        [Test]
        public void Test_Implementation_PropertyRetrieval_Fail()
        {
            // Arrange.
            FailingCache cache = new FailingCache();

            // Act.
            TestDelegate action = () =>
            {
                int value = cache.MyProperty;
            };

            // Assert.
            Assert.Catch<LazyDictionaryException>(action, "Expected the unimplemented retrieval of cached item to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether property retrieval can be done properly and with feedback.
        /// It expects a cached value to be returned if the retrieval is properly.
        /// </summary>
        [Test]
        public void Test_Implementation_PropertyRetrieval_Succes()
        {
            // Arrange.
            SucceedingCache cache = new SucceedingCache();

            // Act.
            int value = cache.MyProperty;

            // Assert.
            Assert.AreEqual(SucceedingCache.CACHED_VALUE, value, "Expected the value to be retrieved from the cache.");
        }
    }
}

#endif